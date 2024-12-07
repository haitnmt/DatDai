using Haihv.DatDai.Lib.Identity.Data.Entities;
using Haihv.DatDai.Lib.Identity.Ldap.Entities;
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
    Task<Result<User>> CreateOrUpdateAsync(UserLdap? userLdap, string? password = null);

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
    Task<List<User>> GetByAuthenticationTypeAsync(int authenticationType = 1);

    /// <summary>
    /// Lấy thông tin người dùng theo UserName.
    /// </summary>
    /// <param name="username">
    /// Tên đăng nhập của người dùng.
    /// </param>
    /// <param name="distinguishedName">
    /// DistinguishedName của người dùng.
    /// </param>
    Task<User?> GetAsync(string? username = null,
        string? distinguishedName = null);
    /// <summary>
    /// Đăng ký danh sách nhóm của người dùng.
    /// </summary>
    /// <param name="userLdap">Thông tin người dùng từ LDAP.</param>
    Task RegisterUserGroupAsync(UserLdap userLdap);
}
