using Audit.Core;
using Haihv.DatDai.Lib.Extension.Configuration.PostgreSQL;
using Haihv.DatDai.Lib.Identity.Data.Entities;
using Haihv.DatDai.Lib.Identity.Data.Extensions;
using Haihv.DatDai.Lib.Identity.Data.Interfaces;
using Haihv.DatDai.Lib.Identity.Ldap.Entities;
using LanguageExt.Common;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Haihv.DatDai.Lib.Identity.Data.Services;

public sealed class UserService(
    ILogger logger,
    PostgreSqlConnection postgreSqlConnection,
    AuditDataProvider? auditDataProvider) : IUserService
{
    private readonly IdentityDbContext _dbContextRead =
        new(postgreSqlConnection.ReplicaConnectionString, auditDataProvider);

    private readonly IdentityDbContext _dbContextWrite =
        new(postgreSqlConnection.PrimaryConnectionString, auditDataProvider);

    /// <summary>
    /// Tạo hoặc cập nhật thông tin người dùng.
    /// </summary>
    /// <param name="userLdap">Đối tượng người dùng từ LDAP.</param>
    /// <param name="password">Mật khẩu người dùng (tùy chọn).</param>
    /// <returns>Kết quả chứa đối tượng người dùng hoặc lỗi.</returns>
    public async Task<Result<User>> CreateOrUpdateAsync(UserLdap? userLdap, string? password = null)
    {
        if (userLdap is null)
        {
            return new Result<User>(new ArgumentNullException(nameof(userLdap)));
        }

        try
        {
            var user = userLdap.ToUser();
            var existingUser = await _dbContextRead.Users
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(u =>
                u.Id == user.Id);
            if (existingUser is not null)
            {
                if (existingUser.UpdatedAt == user.UpdatedAt && existingUser.HashUser() == user.HashUser())
                {
                    return existingUser;
                }

                existingUser.UserName = user.UserName;
                existingUser.Email = user.Email;
                existingUser.DisplayName = user.DisplayName;
                existingUser.DistinguishedName = user.DistinguishedName;
                existingUser.JobTitle = user.JobTitle;
                existingUser.Department = user.Department;
                existingUser.Organization = user.Organization;
                existingUser.IsLocked = user.IsLocked;
                existingUser.IsPwdMustChange = user.IsPwdMustChange;
                existingUser.PwdLastSet = user.PwdLastSet;
                existingUser.GhiChu = user.GhiChu;
                existingUser.CreatedAt = user.CreatedAt;
                existingUser.UpdatedAt = user.UpdatedAt;
                existingUser.IsDeleted = user.IsDeleted;
                existingUser.DeletedAt = user.DeletedAt;
                if (!string.IsNullOrWhiteSpace(password))
                {
                    existingUser.HashPassword = BCrypt.Net.BCrypt.HashPassword(password);
                }
                _dbContextWrite.Users.Update(existingUser);
                
                await _dbContextWrite.SaveChangesAsync();
                return existingUser;
            }

            if (!string.IsNullOrWhiteSpace(password))
            {
                user.HashPassword = BCrypt.Net.BCrypt.HashPassword(password);
            }

            _dbContextWrite.Users.Add(user);
            await _dbContextWrite.SaveChangesAsync();

            return user;
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Error while creating or updating user {UserLdap}", userLdap.Id);
            return new Result<User>(ex);
        }
    }

    /// <summary>
    /// Lấy thông tin người dùng theo tên người dùng và loại người dùng.
    /// </summary>
    /// <param name="authenticationType">Kiểu người dùng.</param>
    /// <remarks>
    /// <c>0: CSDL/SystemUser </c>
    /// <c>1: ADDC/LDAP</c>
    /// <c>2: VneID</c>
    /// <c>3: bacninh.gov.vn</c>
    /// <c>4: Google</c>
    /// <c>5: Microsoft</c>
    /// <c>6: Facebook</c>
    /// <c>7: GitHub</c>
    /// </remarks>
    public async Task<List<User>> GetByAuthenticationTypeAsync(int authenticationType = 1)
    {
        return await _dbContextRead.Users
            .Where(u => u.AuthenticationType == authenticationType)
            .ToListAsync();
    }

    /// <summary>
    /// Lấy thông tin người dùng theo UserName.
    /// </summary>
    /// <param name="username">
    /// Tên đăng nhập của người dùng.
    /// </param>
    /// <param name="distinguishedName">
    /// DistinguishedName của người dùng.
    /// </param>
    public async Task<User?> GetAsync(string? username = null, 
        string? distinguishedName = null)
    {
        if (string.IsNullOrWhiteSpace(username) && string.IsNullOrWhiteSpace(distinguishedName))
        {
            return null;
        }

        if (!string.IsNullOrWhiteSpace(username))
        {
            return await _dbContextRead.Users.FirstOrDefaultAsync(u => u.UserName == username );
        }

        return await _dbContextRead.Users.FirstOrDefaultAsync(u => u.DistinguishedName == distinguishedName);
    }

    /// <summary>
    /// Lấy thông tin người dùng
    /// </summary>
    /// <param name="groupLdap"><see cref="GroupLdap"/>
    /// Nhóm LDAP của người dùng. 
    /// </param>>
    public async Task<List<User>> GetAsync(GroupLdap groupLdap)
    {
        List<User> result = [];
        foreach (var member in groupLdap.GroupMembers)
        {
            var user = await GetAsync(distinguishedName: member);
            if (user is not null)
            {
                result.Add(user);
            }
        }

        return result;
    }

    /// <summary>
    /// Lấy danh sách ID của các nhóm mà user LDAP là thành viên.
    /// </summary>
    /// <param name="userLdap">Thông tin user từ LDAP.</param>
    /// <param name="useWriteDb"> Sử dụng cơ sở dữ liệu ghi để lấy thông tin nhóm.</param>
    /// <returns>Danh sách ID của các nhóm.</returns>
    private async Task<List<Guid>> GetMemberOfAsync(UserLdap userLdap, bool useWriteDb = false)
    {
        try
        {
            var dbContext = useWriteDb ? _dbContextWrite : _dbContextRead;
            return await dbContext.Groups
                .Where(g => g.GroupType == 1 && g.DistinguishedName != null &&
                            userLdap.MemberOf.Contains(g.DistinguishedName))
                .Select(g => g.Id)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            logger.Error(ex, "GetMemberOfAsync");
            return [];
        }
    }

    /// <summary>
    /// Đăng ký danh sách nhóm của người dùng.
    /// </summary>
    /// <param name="userLdap">Thông tin người dùng từ LDAP.</param>
    public async Task RegisterUserGroupAsync(UserLdap userLdap)
    {
        var groupIds = await GetMemberOfAsync(userLdap);
        var userGroupService = new UserGroupService(logger, postgreSqlConnection, auditDataProvider);
        await userGroupService.RegisterAsync(userLdap.Id, groupIds);
    }
}