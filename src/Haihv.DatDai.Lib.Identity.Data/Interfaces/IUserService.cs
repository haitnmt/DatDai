using Haihv.DatDai.Lib.Identity.Data.Entries;
using Haihv.DatDai.Lib.Identity.Ldap.Entries;
using LanguageExt.Common;

namespace Haihv.DatDai.Lib.Identity.Data.Interfaces;

public interface IUserService
{
    /// <summary>
    /// Tạo hoặc cập nhật thông tin người dùng.
    /// </summary>
    /// <param name="userLdap">Đối tượng người dùng từ LDAP.</param>
    /// <param name="password">Mật khẩu người dùng (tùy chọn).</param>
    /// <returns>Kết quả chứa đối tượng người dùng hoặc lỗi.</returns>
    Task<Result<User>> CreateOrUpdateAsync(UserLdap userLdap, string? password = null);
}