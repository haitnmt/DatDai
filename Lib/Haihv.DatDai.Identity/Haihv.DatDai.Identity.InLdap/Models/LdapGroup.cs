namespace Haihv.DatDai.Identity.InLdap.Models;

/// <summary>
/// Đại diện cho một nhóm LDAP.
/// </summary>
public class LdapGroup : ILdap
{
    /// <summary>
    /// GUID của đối tượng LDAP.
    /// </summary>
    public virtual Guid Id { get; init; }

    /// <summary>
    /// Tên phân biệt (Distinguished Name) của nhóm.
    /// </summary>
    public string? DistinguishedName { get; init; }

    /// <summary>
    /// Tên tài khoản SAM của nhóm.
    /// </summary>
    public string? SamAccountName { get; init; }

    /// <summary>
    /// Tên chung (Common Name) của nhóm.
    /// </summary>
    public string? Cn { get; init; }

    /// <summary>
    /// Tập hợp các nhóm mà nhóm này là thành viên.
    /// </summary>
    public HashSet<string> MemberOf { get; init; } = new HashSet<string>();

    /// <summary>
    /// Thời gian tạo nhóm.
    /// </summary>
    public DateTimeOffset WhenCreated { get; init; } = DateTimeOffset.MinValue;

    /// <summary>
    /// Thời gian thay đổi nhóm lần cuối.
    /// </summary>
    public DateTimeOffset? WhenChanged { get; init; } = DateTimeOffset.UtcNow;
}