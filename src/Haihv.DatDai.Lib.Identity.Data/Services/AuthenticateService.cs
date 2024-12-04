using Audit.Core;
using Haihv.DatDai.Lib.Extension.Configuration.PostgreSQL;
using Haihv.DatDai.Lib.Identity.Data.Entries;
using Haihv.DatDai.Lib.Identity.Data.Extensions;
using Haihv.DatDai.Lib.Identity.Ldap;
using Haihv.DatDai.Lib.Identity.Ldap.Entries;
using Haihv.DatDai.Lib.Identity.Ldap.Services;
using LanguageExt;
using LanguageExt.Common;
using LanguageExt.SomeHelp;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Haihv.DatDai.Lib.Identity.Data.Services;

public interface IAuthenticateService
{
    /// <summary>
    /// Xác thực người dùng dựa trên tên đăng nhập và mật khẩu.
    /// </summary>
    /// <param name="username">Tên đăng nhập của người dùng.</param>
    /// <param name="password">Mật khẩu của người dùng.</param>
    /// <returns>Kết quả xác thực, trả về người dùng nếu thành công, ngược lại trả về lỗi.</returns>
    Task<Result<User>> Authenticate(string username, string password);
}

public class AuthenticateService(ILogger logger, 
    PostgreSqlConnection postgreSqlConnection, 
    AuditDataProvider? auditDataProvider,
    ILdapContext ldapContext)
    : IAuthenticateService
{
    private readonly UserService _userService = new(logger, postgreSqlConnection, auditDataProvider);

    private readonly AuthenticateLdapService _authenticateLdapService = new(ldapContext);

    /// <summary>
    /// Xác thực người dùng dựa trên tên đăng nhập và mật khẩu.
    /// </summary>
    /// <param name="username">Tên đăng nhập của người dùng.</param>
    /// <param name="password">Mật khẩu của người dùng.</param>
    /// <returns>Kết quả xác thực, trả về người dùng nếu thành công, ngược lại trả về lỗi.</returns>
    public async Task<Result<User>> Authenticate(string username, string password)
    {
        try
        {
            var user = await _userService.GetByUserNameAsync(username);
            if (user is not null && BCrypt.Net.BCrypt.Verify(password, user.HashPassword)) return user;
            if (ldapContext.CheckUserLdap(username))
            {
                var resultUserLdap = _authenticateLdapService.Authenticate(username, password);
                var result = resultUserLdap.Map<Result<User>>(userLdap => user = _userService.CreateOrUpdateAsync(userLdap).Result.Map());
            }
            else
            {
                return new Result<User>(new Exception("Tài khoản hoặc mật khẩu không đúng!"));
            }

            return user;
        }
        catch (Exception ex)
        {
            return new Result<User>(ex);
        }
    }
}