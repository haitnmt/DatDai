using Audit.Core;
using Haihv.DatDai.Lib.Extension.Configuration.PostgreSQL;
using Haihv.DatDai.Lib.Identity.Data.Entries;
using LanguageExt.Common;
using Microsoft.EntityFrameworkCore;

namespace Haihv.DatDai.Lib.Identity.Data.Services;

public interface IAuthenticateService
{
    /// <summary>
    /// Xác thực người dùng dựa trên tên đăng nhập và mật khẩu.
    /// </summary>
    /// <param name="username">Tên đăng nhập của người dùng.</param>
    /// <param name="password">Mật khẩu của người dùng.</param>
    /// <returns>Kết quả xác thực, trả về người dùng nếu thành công, ngược lại trả về lỗi.</returns>
    Task<Result<User>> Authenticate(string username, string password);
}

public class AuthenticateService(PostgreSqlConnection postgreSqlConnection, AuditDataProvider? auditDataProvider)
    : IAuthenticateService
{
    private readonly IdentityDbContext _dbContextRead =
        new(postgreSqlConnection.ReplicaConnectionString, auditDataProvider);

    /// <summary>
    /// Xác thực người dùng dựa trên tên đăng nhập và mật khẩu.
    /// </summary>
    /// <param name="username">Tên đăng nhập của người dùng.</param>
    /// <param name="password">Mật khẩu của người dùng.</param>
    /// <returns>Kết quả xác thực, trả về người dùng nếu thành công, ngược lại trả về lỗi.</returns>
    public async Task<Result<User>> Authenticate(string username, string password)
    {
        try
        {
            var user = await _dbContextRead.Users.SingleOrDefaultAsync(x => x.UserName == username);
            if (user is null || !BCrypt.Net.BCrypt.Verify(password, user.HashPassword))
            {
                return new Result<User>(new Exception("Tài khoản hoặc mật khẩu không đúng!"));
            }
            return user;
        }
        catch (Exception ex)
        {
            return new Result<User>(ex);
        }
    }
}