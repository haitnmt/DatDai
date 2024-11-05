namespace Haihv.DatDai.IdentityApi.Interface;

/// <summary>
/// Giao diện người dùng kế thừa từ ILdapUser.
/// </summary>
public interface IUser : ILdapUser
{
    /// <summary>
    /// Mật khẩu của người dùng.
    /// </summary>
    /// <remarks>
    /// Mật khẩu được mã hóa bằng BCrypt.Net.
    /// </remarks>
    string? HashPassword { get; set; }
    string? GhiChu { get; set; }

    /// <summary>
    /// Kiểu xác thực của người dùng.
    /// </summary>
    /// <remarks>
    /// <c>0: CSDL</c>
    /// <c>1: ADDC/LDAP</c>
    /// <c>2: VneID</c>
    /// <c>3: bacninh.gov.vn</c>
    /// <c>4: Google</c>
    /// <c>5: Microsoft</c>
    /// <c>6: Facebook</c>
    /// <c>7: GitHub</c>
    /// </remarks>
    int AuthenticationType { get; init; }
}