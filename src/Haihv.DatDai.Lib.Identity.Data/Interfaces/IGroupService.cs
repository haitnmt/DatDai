using Haihv.DatDai.Lib.Identity.Data.Entities;
using Haihv.DatDai.Lib.Identity.Ldap.Entities;
using LanguageExt.Common;

namespace Haihv.DatDai.Lib.Identity.Data.Interfaces;

public interface IGroupService
{
    /// <summary>
    /// Tạo hoặc cập nhật một nhóm từ thông tin LDAP.
    /// </summary>
    /// <param name="groupLdap">Thông tin nhóm từ LDAP.</param>
    /// <returns>Kết quả chứa nhóm đã tạo hoặc cập nhật hoặc lỗi..</returns>
    Task<Result<Group>> CreateOrUpdateAsync(GroupLdap groupLdap);

    /// <summary>
    /// Tạo hoặc cập nhật danh sách các nhóm từ thông tin LDAP.
    /// </summary>
    /// <param name="groupLdaps">Danh sách thông tin nhóm từ LDAP.</param>
    Task CreateOrUpdateAsync(List<GroupLdap> groupLdaps);
    /// <summary>
    /// Lấy thời gian đồng bộ cuối cùng của nhóm theo loại nhóm.
    /// </summary>
    /// <param name="groupType">Loại nhóm.</param>
    /// <returns>Thời gian đồng bộ cuối cùng của nhóm hoặc null nếu không có.</returns>
    DateTimeOffset? GetLastSyncTime(int groupType);

    /// <summary>
    /// Lấy thông tin nhóm theo tên nhóm và loại nhóm.
    /// </summary>
    /// <param name="groupName">Tên nhóm.</param>
    /// <param name="groupType">Loại nhóm (mặc định là 1).
    /// <c>0: CSDL/SystemUser </c>
    /// <c>1: ADDC/LDAP</c>
    /// </param>
    /// <returns>Kết quả chứa thông tin nhóm hoặc lỗi.</returns>
    Task<Result<Group>> GetGroupByGroupNameAsync(string groupName, int groupType = 1);
    /// <summary>
    /// Xóa mềm các nhóm theo danh sách DistinguishedName.
    /// </summary>
    /// <param name="distinguishedNames">Danh sách DistinguishedName của các nhóm cần xóa.</param>
    /// <param name="notIn">Nếu <c>true</c>, xóa các nhóm không có trong danh sách DistinguishedName;
    /// nếu <c>false</c>, xóa các nhóm có trong danh sách DistinguishedName.</param>
   Task DeleteByDistinctNameAsync(List<string> distinguishedNames, bool notIn = true);
}