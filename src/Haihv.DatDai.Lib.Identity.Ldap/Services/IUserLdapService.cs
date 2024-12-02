using Haihv.DatDai.Lib.Identity.Ldap.Entries;

namespace Haihv.DatDai.Lib.Identity.Ldap.Services;

public interface IUserLdapService
{
    /// <summary>
    /// Lấy thông tin người dùng từ LDAP.
    /// </summary>
    /// <param name="userPrincipalName">Tên người dùng cần lấy thông tin.</param>
    /// <returns>Đối tượng UserLdap chứa thông tin người dùng.</returns>
    Task<UserLdap> GetUserLdapAsync(string userPrincipalName);
}