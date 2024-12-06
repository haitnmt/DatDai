using Audit.Core;
using Haihv.DatDai.Lib.Extension.Configuration.PostgreSQL;
using Haihv.DatDai.Lib.Identity.Data.Entries;
using Haihv.DatDai.Lib.Identity.Data.Extensions;
using Haihv.DatDai.Lib.Identity.Data.Interfaces;
using Haihv.DatDai.Lib.Identity.Ldap.Entries;
using LanguageExt.Common;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Haihv.DatDai.Lib.Identity.Data.Services;

public class GroupService(
    ILogger logger,
    PostgreSqlConnection postgreSqlConnection,
    AuditDataProvider? auditDataProvider) : IGroupService
{
    private readonly IdentityDbContext _dbContextRead =
        new(postgreSqlConnection.ReplicaConnectionString, auditDataProvider);

    private readonly IdentityDbContext _dbContextWrite =
        new(postgreSqlConnection.PrimaryConnectionString, auditDataProvider);

    /// <summary>
    /// Tạo hoặc cập nhật một nhóm từ thông tin LDAP.
    /// </summary>
    /// <param name="groupLdap">Thông tin nhóm từ LDAP.</param>
    /// <returns>Kết quả chứa nhóm đã tạo hoặc cập nhật hoặc lỗi..</returns>
    public async Task<Result<Group>> CreateOrUpdateAsync(GroupLdap groupLdap)
    {
        try
        {
            var userGroupService = new UserGroupService(logger, postgreSqlConnection, auditDataProvider);
            var group = groupLdap.ToGroup();
            group.MemberOf = await GetMemberOfAsync(groupLdap, true);
            var existingGroup = await _dbContextWrite.Groups.FirstOrDefaultAsync(g => g.Id == group.Id ||
                (g.GroupName == group.GroupName && g.GroupType == group.GroupType));
            if (existingGroup is not null)
            {
                if (existingGroup.UpdatedAt == group.UpdatedAt && existingGroup.HashGroup() == group.HashGroup())
                {
                    return existingGroup;
                }

                existingGroup.GroupName = group.GroupName;
                existingGroup.GroupType = group.GroupType;
                existingGroup.CreatedAt = group.CreatedAt;
                existingGroup.UpdatedAt = group.UpdatedAt;
                _dbContextWrite.Groups.Update(existingGroup);
                await _dbContextWrite.SaveChangesAsync();
                _ = userGroupService.UpdatedAsync(groupLdap);
                return existingGroup;
            }

            _dbContextWrite.Groups.Add(group);
            await _dbContextWrite.SaveChangesAsync();
            return group;
        }
        catch (Exception ex)
        {
            logger.Error(ex, "CreateOrUpdateAsync");
            return new Result<Group>(ex);
        }
    }

    /// <summary>
    /// Tạo hoặc cập nhật danh sách các nhóm từ thông tin LDAP.
    /// </summary>
    /// <param name="groupLdaps">Danh sách thông tin nhóm từ LDAP.</param>
    public async Task CreateOrUpdateAsync(List<GroupLdap> groupLdaps)
    {
        try
        {
            var userGroupService = new UserGroupService(logger, postgreSqlConnection, auditDataProvider);
            foreach (var groupLdap in groupLdaps)
            {
                var group = groupLdap.ToGroup();
                group.MemberOf = await GetMemberOfAsync(groupLdap, true);
                var existingGroup = await _dbContextRead.Groups.FirstOrDefaultAsync(g => g.Id == group.Id ||
                    (g.GroupName == group.GroupName && g.GroupType == group.GroupType));
                if (existingGroup is not null)
                {
                    if (existingGroup.UpdatedAt == group.UpdatedAt &&
                        existingGroup.HashGroup() == group.HashGroup())
                    {
                        continue;
                    }
            
                    existingGroup.GroupName = group.GroupName;
                    existingGroup.DistinguishedName = group.DistinguishedName;
                    existingGroup.GroupType = group.GroupType;
                    existingGroup.GhiChu = group.GhiChu;
                    existingGroup.CreatedAt = group.CreatedAt;
                    existingGroup.UpdatedAt = group.UpdatedAt;
                    _dbContextWrite.Groups.Update(existingGroup);
                }
                else
                {
                    _dbContextWrite.Groups.Add(group);
                }

                await _dbContextWrite.SaveChangesAsync();
                
                _ = userGroupService.UpdatedAsync(groupLdap);
            }
        }
        catch (Exception ex)
        {
            logger.Error(ex, "CreateOrUpdateAsync");
        }
    }

    /// <summary>
    /// Lấy thời gian đồng bộ cuối cùng của nhóm theo loại nhóm.
    /// </summary>
    /// <param name="groupType">Loại nhóm.</param>
    /// <returns>Thời gian đồng bộ cuối cùng của nhóm hoặc null nếu không có.</returns>
    public DateTimeOffset? GetLastSyncTime(int groupType)
    {
        return _dbContextRead.Groups
            .Where(g => g.GroupType == groupType)
            .Select(g => g.UpdatedAt)
            .AsEnumerable()
            .DefaultIfEmpty(DateTimeOffset.MinValue)
            .Max();
    }

    /// <summary>
    /// Lấy thông tin nhóm theo tên nhóm và loại nhóm.
    /// </summary>
    /// <param name="groupName">Tên nhóm.</param>
    /// <param name="groupType">Loại nhóm (mặc định là 1).
    /// <c>0: CSDL/SystemUser </c>
    /// <c>1: ADDC/LDAP</c>
    /// </param>
    /// <returns>Kết quả chứa thông tin nhóm hoặc lỗi.</returns>
    public async Task<Result<Group>> GetGroupByGroupNameAsync(string groupName, int groupType = 1)
    {
        try
        {
            var group = await _dbContextRead.Groups.FirstOrDefaultAsync(g =>
                g.GroupName == groupName && g.GroupType == groupType);
            return group ?? new Result<Group>(new Exception($"Group {groupName} not found"));
        }
        catch (Exception ex)
        {
            logger.Error(ex, "GetGroupByGroupNameAsync");
            return new Result<Group>(ex);
        }
    }

    /// <summary>
    /// Lấy danh sách ID của các nhóm mà nhóm LDAP là thành viên.
    /// </summary>
    /// <param name="groupLdap">Thông tin nhóm từ LDAP.</param>
    /// <param name="useWriteDb"> Sử dụng cơ sở dữ liệu ghi để lấy thông tin nhóm.</param>
    /// <returns>Danh sách ID của các nhóm.</returns>
    private async Task<List<Guid>> GetMemberOfAsync(GroupLdap groupLdap, bool useWriteDb = false)
    {
        try
        {
            var dbContext = useWriteDb ? _dbContextWrite : _dbContextRead;
            return await dbContext.Groups
                .Where(g => g.GroupType == 1 && g.DistinguishedName != null &&
                            groupLdap.MemberOf.Contains(g.DistinguishedName))
                .Select(g => g.Id)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            logger.Error(ex, "GetMemberOfAsync");
            return [];
        }
    }
}