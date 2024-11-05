namespace Haihv.DatDai.IdentityApi.Interface;

/// <summary>
/// Giao diện người dùng kế thừa từ ILdap.
/// </summary>
public interface ILdapUser : ILdap
{
    /// <summary>
    /// Tên chính của người dùng.
    /// </summary>
    string? UserPrincipalName { get; init; }

    /// <summary>
    /// Email của người dùng.
    /// </summary>
    string? Email { get; init; }

    /// <summary>
    /// Tên hiển thị của người dùng.
    /// </summary>
    string? DisplayName { get; init; }

    /// <summary>
    /// Chức danh công việc của người dùng.
    /// </summary>
    string? JobTitle { get; set; }

    /// <summary>
    /// Mô tả về người dùng.
    /// </summary>
    string? Description { get; init; }

    /// <summary>
    /// Phòng ban của người dùng.
    /// </summary>
    string? Department { get; init; }

    /// <summary>
    /// Tổ chức của người dùng.
    /// </summary>
    string? Organization { get; init; }

    /// <summary>
    /// URL miền của người dùng.
    /// </summary>
    string? DomainUrl { get; init; }

    /// <summary>
    /// Trạng thái khóa của người dùng.
    /// </summary>
    bool IsLocked { get; init; }

    /// <summary>
    /// Trạng thái yêu cầu thay đổi mật khẩu của người dùng.
    /// </summary>
    bool IsPwdMustChange { get; init; }

    /// <summary>
    /// Thời gian mật khẩu được đặt lần cuối.
    /// </summary>
    DateTime PwdLastSet { get; init; }
}