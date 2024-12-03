using Haihv.DatDai.Lib.Identity.Ldap.Entries;

namespace Haihv.DatDai.Lib.Identity.Ldap.Interfaces;

public interface IGroupLdapService
{
    Task<bool> ExistsAsync(Guid objectGuid);

    Task<GroupLdap?> GetDetailGroupsLdapModelByGroupDnAsync(string distinguishedName,
        DateTime whenChanged = default);

    Task<GroupLdap?> GetRootGroupAsync();
    /// <summary>
    /// Lấy tất cả các nhóm LDAP đã thay đổi kể từ thời gian được chỉ định.
    /// </summary>
    /// <param name="whenChanged">
    /// Thời gian để lọc các nhóm theo thời gian thay đổi cuối cùng của chúng (lớn hơn giá trị nhập vào 1s).
    /// Mặc định là <see cref="DateTime.MinValue"/>. Sẽ bỏ qua không lọc theo điều kiện này nếu giá trị là mặc định.
    /// </param>
    /// <returns>Danh sách các đối tượng <see cref="GroupLdap"/> đại diện cho các nhóm LDAP.</returns>
    Task<List<GroupLdap>> GetAllGroupsLdapAsync(DateTime whenChanged = default);

    /// <summary>
    /// Lấy tất cả các nhóm LDAP đã thay đổi kể từ ngày chỉ định, tìm theo từng tầng dựa trên memberOf.
    /// </summary>
    /// <param name="whenChanged">
    /// Ngày để lọc các nhóm theo ngày thay đổi cuối cùng của chúng (lớn hơn giá trị nhập vào 1s).
    /// Mặc định là <see cref="DateTime.MinValue"/>.
    /// </param>
    /// <returns>Danh sách các đối tượng <see cref="GroupLdap"/> đại diện cho các nhóm LDAP.</returns>
    Task<List<GroupLdap>> GetAllGroupsLdapByRecursiveAsync(DateTime whenChanged = default);
}