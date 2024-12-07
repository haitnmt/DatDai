using Audit.Core;
using Haihv.DatDai.Lib.Extension.Configuration.PostgreSQL;
using Haihv.DatDai.Lib.Extension.String;
using Haihv.DatDai.Lib.Identity.Data.Entities;
using Haihv.DatDai.Lib.Identity.Data.Interfaces;
using Haihv.DatDai.Lib.Identity.Ldap;
using Haihv.DatDai.Lib.Identity.Ldap.Services;
using Haihv.DatDai.Lib.Model.Request.Identity;
using LanguageExt.Common;
using Microsoft.Extensions.Caching.Hybrid;
using Serilog;

namespace Haihv.DatDai.Lib.Identity.Data.Services;

public interface IAuthenticateService
{
    /// <summary>
    /// Xác thực người dùng dựa trên tên đăng nhập và mật khẩu.
    /// </summary>
    /// <param name="loginRequest">
    /// Thông tin đăng nhập của người dùng, bao gồm tên đăng nhập và mật khẩu.
    /// </param>
    /// <returns>Kết quả xác thực, trả về người dùng nếu thành công, ngược lại trả về lỗi.</returns>
    Task<Result<Response>> Authenticate(LoginRequest loginRequest);
}

public sealed record Response(string AccessToken, Guid ClientId, string RefreshToken);

public sealed class AuthenticateService(
    ILogger logger,
    HybridCache cache,
    IUserService userService,
    IRefreshTokensService refreshTokensService,
    TokenProvider tokenProvider,
    ILdapContext ldapContext)
    : IAuthenticateService
{
    private readonly AuthenticateLdapService _authenticateLdapService = new(logger, ldapContext);

    /// <summary>
    /// Xác thực người dùng dựa trên tên đăng nhập và mật khẩu.
    /// </summary>
    /// <param name="loginRequest">Tên đăng nhập của người dùng.</param>
    /// <returns>Kết quả xác thực, trả về người dùng nếu thành công, ngược lại trả về lỗi.</returns>
    public async Task<Result<Response>> Authenticate(LoginRequest loginRequest)
    {
        if (string.IsNullOrWhiteSpace(loginRequest.Username) || string.IsNullOrWhiteSpace(loginRequest.Password))
            return new Result<Response>(new Exception("Tên đăng nhập hoặc mật khẩu không được để trống!"));
        var username = loginRequest.Username;
        try
        {
            // Kiểm tra xem người dùng có phải là người dùng LDAP không
            var isUserLdap = ldapContext.CheckUserLdap(username);

            // Nếu không phải người dùng LDAP thì convert username sang UPN
            if (isUserLdap)
                username = ldapContext.GetUserPrincipalName(username);

            // Xác thực người dùng trong CSDL
            var resultUser = await AuthenticateInData(username, loginRequest.Password);

            return resultUser.Match(
                // Nếu xác thực CSDL thành công thì trả về người dùng
                CreateToken,
                // Nếu xác thực CSDL thất bại thì xác thực trong LDAP
                exUser => !isUserLdap ? new Result<Response>(exUser) : 
                    CreateTokenByUserLdap(username, loginRequest.Password));
        }
        catch (Exception ex)
        {
            return new Result<Response>(ex);
        }
    }

    /// <summary>
    /// Tạo token cho người dùng.
    /// </summary>
    /// <param name="user">Người dùng cần tạo token.</param>
    /// <returns>Kết quả tạo token, trả về token nếu thành công, ngược lại trả về lỗi.</returns>
    private Result<Response> CreateToken(User user)
    {
        var resultRefreshToken = refreshTokensService.CreateToken(user.Id);
        return resultRefreshToken.Match(
            // Nếu tạo RefreshToken thành công thì tạo AccessToken
            refreshToken => new Response(tokenProvider.GenerateToken(user), refreshToken.Id, refreshToken.Token),
            // Nếu tạo RefreshToken thất bại thì trả về lỗi
            ex => new Result<Response>(ex));
    }
    
    /// <summary>
    /// Tạo token cho người dùng LDAP.
    /// </summary>
    /// <param name="username">Tên đăng nhập của người dùng.</param>
    /// <param name="password">Mật khẩu của người dùng.</param>
    /// <returns>Kết quả xác thực, trả về token nếu thành công, ngược lại trả về lỗi.</returns>
    private Result<Response> CreateTokenByUserLdap(string username, string password)
    {
        var resultUserLdap = _authenticateLdapService.Authenticate(username, password);
        return resultUserLdap.Match(
            // Nếu xác thực LDAP thành công thì tạo hoặc cập nhật người dùng trong CSDL
            userLdap =>
            {
                var resultUser = userService.CreateOrUpdateAsync(userLdap, password).Result;
                // Đăng ký nhóm cho người dùng
                userService.RegisterUserGroupAsync(userLdap).Wait();
                            
                return resultUser.Match(
                    CreateToken,
                    // Nếu tạo hoặc cập nhật người dùng thất bại thì trả về lỗi
                    ex => new Result<Response>(ex));
            },
            // Nếu xác thực LDAP thất bại thì xác thực trong CSDL
            exLdap => new Result<Response>(exLdap));
    }
    
    /// <summary>
    /// Xác thực người dùng trong cơ sở dữ liệu.
    /// </summary>
    /// <param name="username">Tên đăng nhập của người dùng.</param>
    /// <param name="password">Mật khẩu của người dùng.</param>
    /// <returns>Kết quả xác thực, trả về người dùng nếu thành công, ngược lại trả về lỗi.</returns>
    private async Task<Result<User>> AuthenticateInData(string username, string password)
    {
        // Xác thực trong CSDL
        var user = await userService.GetAsync(username: username);
        if (user?.HashPassword is null || !await VerifyPassword(username, password, user.HashPassword))
            return new Result<User>(new Exception("Tên đăng nhập hoặc mật khẩu không chính xác!"));
        return user;
    }

    /// <summary>
    /// Xác minh mật khẩu của người dùng.
    /// </summary>
    /// <param name="username">Tên đăng nhập của người dùng.</param>
    /// <param name="password">Mật khẩu của người dùng.</param>
    /// <param name="hashPassword">Mật khẩu đã được băm.</param>
    /// <returns>Trả về true nếu mật khẩu hợp lệ, ngược lại trả về false.</returns>
    private async Task<bool> VerifyPassword(string username, string password, string? hashPassword)
    {
        // Nếu hashPassword nếu null hoặc rỗng thì trả về false
        if (string.IsNullOrWhiteSpace(hashPassword))
            return false;
        var keyData = $"{username}:{password.ComputeHash()}:{hashPassword}";
        var cacheKey = keyData.ComputeHash() ?? keyData;
        return await cache.GetOrCreateAsync(cacheKey, _ =>
        {
            var verify = BCrypt.Net.BCrypt.Verify(password, hashPassword);
            return ValueTask.FromResult(verify);
        });
    }
}