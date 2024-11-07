namespace Haihv.DatDai.Identity.InLdap.Models;

/// <summary>
/// Lớp đại diện cho người dùng LDAP.
/// </summary>
public class LdapUser : ILdap
{
    /// <summary>
    /// GUID của người dùng.
    /// </summary>
    public virtual Guid Id { get; init; }

    /// <summary>
    /// Tên phân biệt của người dùng.
    /// </summary>
    public string? DistinguishedName { get; init; }

    /// <summary>
    /// Tên tài khoản SAM của người dùng.
    /// </summary>
    public string? SamAccountName { get; init; }

    /// <summary>
    /// Tên chung của người dùng.
    /// </summary>
    public string? Cn { get; init; }

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

    /// <summary>
    /// Các nhóm mà người dùng là thành viên.
    /// </summary>
    public HashSet<string> MemberOf { get; init; } = [];

    /// <summary>
    /// Thời gian tạo người dùng.
    /// </summary>
    public DateTimeOffset WhenCreated { get; init; } = DateTimeOffset.MinValue;

    /// <summary>
    /// Thời gian thay đổi người dùng lần cuối.
    /// </summary>
    public DateTimeOffset? WhenChanged { get; init; } = DateTimeOffset.UtcNow;
}