using Haihv.DatDai.Lib.Identity.Ldap.Entries;

namespace Haihv.DatDai.Lib.Identity.Ldap.Interfaces;

public interface IUserLdapService
{
    /// <summary>
    /// Lấy thông tin người dùng từ LDAP.
    /// </summary>
    /// <param name="userPrincipalName">Tên người dùng cần lấy thông tin.</param>
    /// <param name="whenChanged"></param>
    /// <returns>Đối tượng UserLdap chứa thông tin người dùng.</returns>
    Task<UserLdap?> GetByPrincipalNameAsync(string userPrincipalName, DateTime whenChanged = default);

    /// <summary>
    /// Lấy thông tin người dùng từ LDAP.
    /// </summary>
    /// <param name="distinguishedName">
    /// Tên định danh của người dùng cần lấy thông tin.
    /// </param>
    /// <param name="whenChanged"></param>
    /// <returns>Đối tượng UserLdap chứa thông tin người dùng.</returns>
    Task<UserLdap?> GetByDistinctNameAsync(string distinguishedName, DateTime whenChanged = default);
}