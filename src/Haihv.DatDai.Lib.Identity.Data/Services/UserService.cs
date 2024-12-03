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

public class UserService(
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
    public async Task<Result<User>> CreateOrUpdateAsync(UserLdap userLdap, string? password = null)
    {
        try
        {
            var user = userLdap.ToUser();
            var existingUser = await _dbContextRead.Users.FirstOrDefaultAsync(u =>
                u.Id == user.Id || 
                (u.UserName == user.UserName && u.AuthenticationType == user.AuthenticationType));
            if (existingUser is not null)
            {
                if (existingUser.WhenChanged == user.WhenChanged && existingUser.HashUser() == user.HashUser())
                {
                    return existingUser;
                }

                existingUser.UserName = user.UserName;
                existingUser.Email = user.Email;
                existingUser.DisplayName = user.DisplayName;
                existingUser.JobTitle = user.JobTitle;
                existingUser.Description = user.Description;
                existingUser.Department = user.Department;
                existingUser.Organization = user.Organization;
                existingUser.IsLocked = user.IsLocked;
                existingUser.IsPwdMustChange = user.IsPwdMustChange;
                existingUser.PwdLastSet = user.PwdLastSet;
                existingUser.WhenCreated = user.WhenCreated;
                existingUser.WhenChanged = user.WhenChanged;
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
}