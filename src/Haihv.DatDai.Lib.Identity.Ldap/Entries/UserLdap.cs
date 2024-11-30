namespace Haihv.DatDai.Lib.Identity.Ldap.Entries;

/// <summary>
/// Lớp đại diện cho người dùng LDAP.
/// </summary>
public class UserLdap : BaseLdap
{
    /// <summary>
    /// Email của người dùng.
    /// </summary>
    public string? Email { get; init; }

    /// <summary>
    /// Tên hiển thị của người dùng.
    /// </summary>
    public string? DisplayName { get; init; }

    /// <summary>
    /// Tên chính của người dùng.
    /// </summary>
    public string? UserPrincipalName { get; init; }

    /// <summary>
    /// Chức danh công việc của người dùng.
    /// </summary>
    public string? JobTitle { get; set; }

    /// <summary>
    /// Mô tả về người dùng.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Phòng ban của người dùng.
    /// </summary>
    public string? Department { get; init; }

    /// <summary>
    /// Tổ chức của người dùng.
    /// </summary>
    public string? Organization { get; init; }

    /// <summary>
    /// URL miền của người dùng.
    /// </summary>
    public string? DomainUrl { get; init; }

    /// <summary>
    /// Trạng thái khóa của người dùng.
    /// </summary>
    public bool IsLocked { get; init; }

    /// <summary>
    /// Trạng thái yêu cầu thay đổi mật khẩu của người dùng.
    /// </summary>
    public bool IsPwdMustChange { get; init; }

    /// <summary>
    /// Thời gian mật khẩu được đặt lần cuối.
    /// </summary>
    public DateTime PwdLastSet { get; init; }
}