using Haihv.DatDai.Lib.Identity.Data.Entries;
using LanguageExt.Common;

namespace Haihv.DatDai.Lib.Identity.Data.Interfaces;

public interface IUserGroupService
{
    /// <summary>
    /// Lấy danh sách UserGroup dựa trên User và/hoặc Group.
    /// </summary>
    /// <param name="user">Đối tượng User</param>
    /// <param name="group">Đối tượng Group</param>
    /// <returns>Kết quả chứa danh sách UserGroup.</returns>
    Task<Result<List<UserGroup>>> GetAsync(User user, Group group);

    Task<Result<List<UserGroup>>> GetAsync(User user);
    Task<Result<List<UserGroup>>> GetAsync(Group group);

    /// <summary>
    /// Lấy danh sách UserGroup dựa trên UserId.
    /// </summary>
    /// <param name="userId">Id của User.</param>
    /// <returns>Kết quả chứa danh sách UserGroup.</returns>
    Task<List<Guid>> GetGroupIdsByUserIdAsync(Guid userId);

    Task<Result<UserGroup>> CreateOrUpdateAsync(UserGroup userGroup);
    Task<Result<UserGroup>> CreateOrUpdateAsync(User user, Group group);
    Task<Result<UserGroup>> CreateOrUpdateAsync(Guid userId, Guid groupId);

    /// <summary>
    /// Đăng ký danh sách UserGroup dựa trên UserId và danh sách GroupId.
    /// </summary>
    /// <param name="userId">Id của User.</param>
    /// <param name="groupIds">Danh sách Id của Group.</param>
    /// <returns>Kết quả chứa danh sách UserGroup đã đăng ký.</returns>
    /// <remarks>
    /// Sẽ xoá các UserGroup của userId có GroupId không nằm trong groupIds.
    /// </remarks>
    Task<Result<List<UserGroup>>> RegisterAsync(Guid userId, List<Guid> groupIds);

    Task DeleteAsync(List<UserGroup> userGroups);
}