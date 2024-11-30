using Haihv.DatDai.Lib.Identity.Ldap.Entries;
using LanguageExt;
using LanguageExt.Common;

namespace Haihv.DatDai.Lib.Identity.Ldap.Services;

public static class AuthenticateService
{
    public static Result<UserLdap> Authenticate(
        this ILdapContext ldapContext, 
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
        // Sử dụng Try để bắt ngoại lệ một cách functional
        try
        {
            // Chuyển đổi username thành UPN
            var userPrincipalName = username.GetUserPrincipalName(ldapConnectionInfo);
            // Thực hiện xác thực
            ldapContext.Connection.Bind(
                new System.Net.NetworkCredential(userPrincipalName, password)
            );
            return new UserLdap
            {
                UserPrincipalName = userPrincipalName
            };
        }
        catch (Exception ex)
        {
            return new Result<UserLdap>(ex);
        }
    }
    private static string GetUserPrincipalName(this string userName, LdapConnectionInfo ldapConnectionInfo)
    {
        userName = new string(userName.Where(c => char.IsLetterOrDigit(c) || c == '\\').ToArray());
        return $"{(userName.Replace($"{ldapConnectionInfo.Domain}\\", "")).Trim()}@{ldapConnectionInfo.DomainFullname}";
    }
}