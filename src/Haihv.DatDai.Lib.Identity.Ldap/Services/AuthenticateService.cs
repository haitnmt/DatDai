using Haihv.DatDai.Lib.Identity.Ldap.Entries;
using LanguageExt.Common;

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
/// <param name="userLdapService">Dịch vụ người dùng LDAP.</param>
public class AuthenticateLdapService(ILdapContext ldapContext, 
    IUserLdapService userLdapService) : IAuthenticateLdapService
{
    /// <summary>
    /// Xác thực người dùng với tên đăng nhập và mật khẩu.
    /// </summary>
    /// <param name="username">Tên đăng nhập của người dùng.</param>
    /// <param name="password">Mật khẩu của người dùng.</param>
    /// <returns>Kết quả xác thực người dùng LDAP.</returns>
    public Result<UserLdap> Authenticate(
        string username,
        string password)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            return new Result<UserLdap>(new Exception("Tài khoản hoặc mật khẩu trống"));
        }

        var ldapConnectionInfo = ldapContext.LdapConnectionInfo;
        if (string.IsNullOrWhiteSpace(ldapConnectionInfo.Host) ||
            string.IsNullOrWhiteSpace(ldapConnectionInfo.DomainFullname) ||
            string.IsNullOrWhiteSpace(ldapConnectionInfo.Domain))
            return new Result<UserLdap>(new Exception("Cấu hình Ldap không hợp lệ"));
        try
        {
            // Chuyển đổi username thành UPN
            var userPrincipalName = ldapContext.GetUserPrincipalName(username);
            // Thực hiện xác thực
            ldapContext.Connection.Bind(
                new System.Net.NetworkCredential(userPrincipalName, password)
            );
            return userLdapService.GetUserLdapAsync(userPrincipalName).Result;
        }
        catch (Exception ex)
        {
            return new Result<UserLdap>(ex);
        }
    }
}