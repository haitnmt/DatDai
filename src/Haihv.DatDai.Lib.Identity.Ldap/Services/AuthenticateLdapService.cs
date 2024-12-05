using System.DirectoryServices.Protocols;
using Haihv.DatDai.Lib.Identity.Ldap.Entries;
using LanguageExt.Common;
using Serilog;

namespace Haihv.DatDai.Lib.Identity.Ldap.Services;

/// <summary>
/// Dịch vụ xác thực người dùng thông qua LDAP.
/// </summary>
public interface IAuthenticateLdapService
{
    Result<UserLdap> Authenticate(
        string username,
        string password);
}

/// <summary>
/// Dịch vụ xác thực người dùng thông qua LDAP.
/// </summary>
/// <param name="ldapContext">Ngữ cảnh LDAP.</param>
public class AuthenticateLdapService(ILogger logger,ILdapContext ldapContext) : IAuthenticateLdapService
{
    private readonly UserLdapService _userLdapService = new (ldapContext);
    /// <summary>
    /// Xác thực người dùng với tên đăng nhập và mật khẩu.
    /// </summary>
    /// <param name="userPrincipalName">
    /// Tên người dùng (tên đăng nhập) của người dùng.
    /// </param>
    /// <param name="password">Mật khẩu của người dùng.</param>
    /// <returns>Kết quả xác thực người dùng LDAP.</returns>
    public Result<UserLdap> Authenticate(string userPrincipalName, string password)
    {
        if (string.IsNullOrWhiteSpace(userPrincipalName) || string.IsNullOrWhiteSpace(password))
        {
            return new Result<UserLdap>(new Exception("Tài khoản hoặc mật khẩu trống"));
        }
        var ldapConnectionInfo = ldapContext.LdapConnectionInfo;
        if (string.IsNullOrWhiteSpace(ldapConnectionInfo.Host) ||
            string.IsNullOrWhiteSpace(ldapConnectionInfo.DomainFullname) ||
            string.IsNullOrWhiteSpace(ldapConnectionInfo.Domain))
        {
            logger.Error("Cấu hình Ldap không hợp lệ {LdapConnectionInfo}", ldapConnectionInfo);
            return new Result<UserLdap>(new Exception("Cấu hình Ldap không hợp lệ"));
        }

        try
        {
            var userLdap = _userLdapService.GetByPrincipalNameAsync(userPrincipalName).Result;
            if (userLdap is null)
            {
                var messenger = $"Người dùng không tồn tại [{userPrincipalName}]";
                logger.Warning(messenger);
                return new Result<UserLdap>(new Exception(messenger));
            }
            // Thực hiện xác thực
            ldapContext.Connection.Bind(
                new System.Net.NetworkCredential(userPrincipalName, password)
            );
            return userLdap;
        }
        catch (Exception ex)
        {
            if (ex is not LdapException { ErrorCode: 49 })
            {
                logger.Error(ex, "Lỗi khi kết nối đến LDAP: {LdapInfo}", ldapContext.ToLogInfo());
            }
            return new Result<UserLdap>(ex);
        }
    }
}