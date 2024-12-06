using Audit.Core;
using Haihv.DatDai.Lib.Extension.Configuration.PostgreSQL;
using Haihv.DatDai.Lib.Extension.String;
using Haihv.DatDai.Lib.Identity.Data.Entries;
using Haihv.DatDai.Lib.Identity.Ldap;
using Haihv.DatDai.Lib.Identity.Ldap.Services;
using Haihv.DatDai.Lib.Model.Request.Identity;
using LanguageExt.Common;
using Microsoft.Extensions.Caching.Memory;
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
    Task<Result<User>> Authenticate(LoginRequest loginRequest);
}

public class AuthenticateService(
    ILogger logger,
    IMemoryCache memoryCache,
    PostgreSqlConnection postgreSqlConnection,
    AuditDataProvider? auditDataProvider,
    ILdapContext ldapContext)
    : IAuthenticateService
{
    private readonly UserService _userService = new(logger, postgreSqlConnection, auditDataProvider);

    private readonly AuthenticateLdapService _authenticateLdapService = new(logger, ldapContext);

    /// <summary>
    /// Xác thực người dùng dựa trên tên đăng nhập và mật khẩu.
    /// </summary>
    /// <param name="loginRequest">Tên đăng nhập của người dùng.</param>
    /// <returns>Kết quả xác thực, trả về người dùng nếu thành công, ngược lại trả về lỗi.</returns>
    public async Task<Result<User>> Authenticate(LoginRequest loginRequest)
    {
        if (string.IsNullOrWhiteSpace(loginRequest.Username) || string.IsNullOrWhiteSpace(loginRequest.Password))
            return new Result<User>(new Exception("Tên đăng nhập hoặc mật khẩu không được để trống!"));
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
                _ => resultUser,
                // Nếu xác thực CSDL thất bại thì xác thực trong LDAP
                _ =>
                {
                    // Nếu không phải người dùng LDAP thì trả về lỗi
                    if (!isUserLdap) return resultUser;
                    // Nếu là người dùng LDAP thì xác thực trong LDAP
                    var resultUserLdap = _authenticateLdapService.Authenticate(username, loginRequest.Password);
                    return resultUserLdap.Match(
                        // Nếu xác thực LDAP thành công thì tạo hoặc cập nhật người dùng trong CSDL
                        userLdap =>
                        {
                            resultUser = _userService.CreateOrUpdateAsync(userLdap, loginRequest.Password).Result;
                            // Đăng ký nhóm cho người dùng
                            _userService.RegisterUserGroupAsync(userLdap).Wait();
                            return resultUser;
                        },
                        // Nếu xác thực LDAP thất bại thì xác thực trong CSDL
                        exLdap => new Result<User>(exLdap));
                });
        }
        catch (Exception ex)
        {
            return new Result<User>(ex);
        }
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
        var user = await _userService.GetAsync(username: username);
        if (user?.HashPassword is null || !VerifyPassword(username, password, user.HashPassword))
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
    private bool VerifyPassword(string username, string password, string? hashPassword)
    {
        // Nếu hashPassword nếu null hoặc rỗng thì trả về false
        if (string.IsNullOrWhiteSpace(hashPassword))
            return false;
        var keyData = $"{username}:{password.ComputeHash()}:{hashPassword}";
        var cacheKey = keyData.ComputeHash() ?? keyData;
        if (memoryCache.TryGetValue(cacheKey, out bool cachedResult))
        {
            return cachedResult;
        }

        var verify = BCrypt.Net.BCrypt.Verify(password, hashPassword);
        if (verify)
        {
            memoryCache.Set(cacheKey, true, new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTimeOffset.Now.AddMonths(1),
                Priority = CacheItemPriority.Low
            });
        }

        return verify;
    }
}