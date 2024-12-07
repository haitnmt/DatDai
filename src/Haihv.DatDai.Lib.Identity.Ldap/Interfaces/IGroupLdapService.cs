using Haihv.DatDai.Lib.Identity.Ldap.Entities;

namespace Haihv.DatDai.Lib.Identity.Ldap.Interfaces;

public interface IGroupLdapService
{
    /// <summary> 
    /// Distinguished Name (DN) của nhóm gốc LDAP.
    /// </summary>
    string RootGroupDn { get; }
    /// <summary>
    /// Kiểm tra sự tồn tại của nhóm LDAP dựa trên Object GUID.
    /// </summary>
    /// <param name="objectGuid">Object GUID của nhóm cần kiểm tra.</param>
    /// <returns>True nếu nhóm tồn tại, ngược lại là False.</returns>
    Task<bool> ExistsAsync(Guid objectGuid);
    /// <summary>
    /// Lấy thông tin nhóm LDAP dựa trên Distinguished Name (DN).
    /// </summary>
    /// <param name="distinguishedName">DN của nhóm cần lấy thông tin.</param>
    /// <param name="whenChanged">
    /// Ngày để lọc các nhóm theo ngày thay đổi cuối cùng của chúng (lớn hơn giá trị nhập vào 1s).
    /// Mặc định là <see cref="DateTime.MinValue"/>.
    /// </param>
    /// <returns>Đối tượng <see cref="GroupLdap"/> đại diện cho nhóm LDAP, hoặc null n��u không tìm thấy.</returns>
    Task<GroupLdap?> GetByDnAsync(string distinguishedName,
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

    /// <summary>
    /// Lấy danh sách các Distinguished Name (DN) của các nhóm LDAP dựa trên thuộc tính memberOf.
    /// </summary>
    /// <param name="memberOf">DN của nhóm cha để tìm các nhóm con.</param>
    /// <param name="whenChanged">
    /// Ngày để lọc các nhóm theo ngày thay đổi cuối cùng của chúng (lớn hơn giá trị nhập vào 1s).
    /// Mặc định là <see cref="DateTime.MinValue"/>.
    /// </param>
    /// <returns>Danh sách các DN của các nhóm LDAP.</returns>
    Task<List<string>> GetDnByMemberOfAsync(string memberOf, DateTime whenChanged = default);
}