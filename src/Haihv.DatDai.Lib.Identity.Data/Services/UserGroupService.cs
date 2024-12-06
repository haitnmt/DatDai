using Audit.Core;
using Haihv.DatDai.Lib.Extension.Configuration.PostgreSQL;
using Haihv.DatDai.Lib.Identity.Data.Entries;
using Haihv.DatDai.Lib.Identity.Data.Interfaces;
using Haihv.DatDai.Lib.Identity.Ldap.Entries;
using LanguageExt.Common;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Haihv.DatDai.Lib.Identity.Data.Services;

public class UserGroupService(ILogger logger,
    PostgreSqlConnection postgreSqlConnection,
    AuditDataProvider? auditDataProvider) : IUserGroupService
{
    private readonly IdentityDbContext _dbContextRead =
        new(postgreSqlConnection.ReplicaConnectionString, auditDataProvider);

    private readonly IdentityDbContext _dbContextWrite =
        new(postgreSqlConnection.PrimaryConnectionString, auditDataProvider);
    
    /// <summary>
    /// Lấy danh sách UserGroup dựa trên User và/hoặc Group.
    /// </summary>
    /// <param name="user">Đối tượng User</param>
    /// <param name="group">Đối tượng Group</param>
    /// <returns>Kết quả chứa danh sách UserGroup.</returns>
    public async Task<Result<List<UserGroup>>> GetAsync(User user, Group group)
    {
        try
        {
            var userGroups = await _dbContextRead.UserGroups
                .Where(ug => ug.UserId == user.Id && ug.GroupId == group.Id).ToListAsync();
            return userGroups;
        }
        catch (Exception ex)
        {
            logger.Error(ex, "GetAsync");
            return new Result<List<UserGroup>>(ex);
        }
    }
    /// <summary>
    /// Lấy danh sách UserGroup dựa trên UserId.
    /// </summary>
    /// <param name="userId">Id của User.</param>
    /// <returns>Kết quả chứa danh sách UserGroup.</returns>
    public async Task<List<Guid>> GetGroupIdsByUserIdAsync(Guid userId)
    {
        try
        {
            var userGroups = await _dbContextRead.UserGroups
                .Where(ug => ug.UserId == userId)
                .Select(x => x.GroupId)
                .ToListAsync();
            return userGroups;
        }
        catch (Exception ex)
        {
            logger.Error(ex, "GetAsync");
            return [];
        }
    }
    
    public async Task<Result<List<UserGroup>>> GetAsync(User user)
    {
        try
        {
            var userGroups = await _dbContextRead.UserGroups.Where(ug => ug.UserId == user.Id).ToListAsync();
            return userGroups;
        }
        catch (Exception ex)
        {
            logger.Error(ex, "GetAsync");
            return new Result<List<UserGroup>>(ex);
        }
    }

    public async Task<Result<List<UserGroup>>> GetAsync(Group group)
    {
        try
        {
            var userGroups = await _dbContextRead.UserGroups.Where(ug => ug.GroupId == group.Id).ToListAsync();
            return userGroups;
        }
        catch (Exception ex)
        {
            logger.Error(ex, "GetAsync");
            return new Result<List<UserGroup>>(ex);
        }
    }
    public async Task<Result<UserGroup>> CreateOrUpdateAsync(UserGroup userGroup)
    {
        try
        {
            var existingUserGroup = await _dbContextWrite.UserGroups.FirstOrDefaultAsync(ug =>
                ug.Id == userGroup.Id ||
                (ug.UserId == userGroup.UserId && ug.GroupId == userGroup.GroupId));
            if (existingUserGroup is not null)
            {
                _dbContextWrite.UserGroups.Update(existingUserGroup);
                await _dbContextWrite.SaveChangesAsync();
                return existingUserGroup;
            }

            _dbContextWrite.UserGroups.Add(userGroup);
            await _dbContextWrite.SaveChangesAsync();
            return userGroup;
        }
        catch (Exception ex)
        {
            logger.Error(ex, "CreateOrUpdateAsync");
            return new Result<UserGroup>(ex);
        }
    }

    public async Task<Result<UserGroup>> CreateOrUpdateAsync(User user, Group group)
        => await CreateOrUpdateAsync(user.Id, group.Id);
    
    public async Task<Result<UserGroup>> CreateOrUpdateAsync(Guid userId, Guid groupId)
    {
        var userGroup = new UserGroup
        {
            UserId = userId,
            GroupId = groupId
        };
        return await CreateOrUpdateAsync(userGroup);
    }

    /// <summary>
    /// Đăng ký danh sách UserGroup dựa trên UserId và danh sách GroupId.
    /// </summary>
    /// <param name="userId">Id của User.</param>
    /// <param name="groupIds">Danh sách Id của Group.</param>
    /// <returns>Kết quả chứa danh sách UserGroup đã đăng ký.</returns>
    /// <remarks>
    /// Sẽ xoá các UserGroup của userId có GroupId không nằm trong groupIds.
    /// </remarks>
    public async Task<Result<List<UserGroup>>> RegisterAsync(Guid userId, List<Guid> groupIds)
    {
        try
        {
            List<UserGroup> result = [];
            foreach (var groupId in groupIds)
            {
                var existingUserGroup = await _dbContextWrite.UserGroups.FirstOrDefaultAsync(ug =>
                    ug.UserId == userId && ug.GroupId == groupId);
                if (existingUserGroup is not null) continue;
                var userGroup = new UserGroup
                {
                    UserId = userId,
                    GroupId = groupId
                };
                result.Add(userGroup);
                _dbContextWrite.UserGroups.Add(userGroup);
            }

            await _dbContextWrite.SaveChangesAsync();
            // Xóa các UserGroup có GroupId không nằm trong groupIds
            var userGroups = await _dbContextWrite.UserGroups
                .Where(ug => ug.UserId == userId && !groupIds.Contains(ug.GroupId)).ToListAsync();
            await DeleteAsync(userGroups);
            return result;
        }
        catch (Exception ex)
        {
            logger.Error(ex, "CreateOrUpdateAsync");
            return new Result<List<UserGroup>>(ex);
        }
    }

    /// <summary>
    /// Cập nhật thông tin từ nhóm LDAP.
    /// </summary>
    /// <param name="groupLdap">Thông tin nhóm LDAP.</param>
    public async Task UpdatedAsync(GroupLdap groupLdap)
    {
        var userIds = _dbContextRead.Users
            .Where(u => u.DistinguishedName != null && groupLdap.UserMembers.Contains(u.DistinguishedName))
            .Select(u => u.Id)
            .ToList();
        List<UserGroup> userGroupsDeletes;
        if (userIds.Count == 0)
        {
            userGroupsDeletes = await _dbContextRead.UserGroups
                .Where(ug => ug.GroupId == groupLdap.Id).ToListAsync();
            await DeleteAsync(userGroupsDeletes);
            return;
        }
        
        userGroupsDeletes = await _dbContextRead.UserGroups
            .Where(ug => ug.GroupId == groupLdap.Id && !userIds.Contains(ug.UserId)).ToListAsync();
        await DeleteAsync(userGroupsDeletes);
        
        foreach (var userGroup in userIds.Select(userId => new UserGroup
                 {
                     UserId = userId,
                     GroupId = groupLdap.Id
                 }))
        {
            await CreateOrUpdateAsync(userGroup);
        }
    }
    
    public async Task DeleteAsync(List<UserGroup> userGroups)
    {   
        if (userGroups.Count == 0) return;
        foreach (var userGroup in userGroups)
        {
            userGroup.IsDeleted = true;
            userGroup.DeletedAtUtc = DateTimeOffset.UtcNow;
            _dbContextWrite.UserGroups.Update(userGroup);
        }
        await _dbContextWrite.SaveChangesAsync();
    }
}