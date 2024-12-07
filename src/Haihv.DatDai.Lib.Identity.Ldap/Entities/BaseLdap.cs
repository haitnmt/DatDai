using Haihv.DatDai.Lib.Identity.Ldap.Interfaces;

namespace Haihv.DatDai.Lib.Identity.Ldap.Entities;

/// <summary>
/// Lớp cơ sở cho các đối tượng LDAP.
/// </summary>
public abstract class BaseLdap : IBaseLdap
{
    /// <summary>
    /// Id duy nhất của đối tượng.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Tên phân biệt của đối tượng.
    /// </summary>
    public string DistinguishedName { get; init; } = string.Empty;

    /// <summary>
    /// Tên tài khoản SAM của đối tượng.
    /// </summary>
    public string? SamAccountName { get; init; }

    /// <summary>
    /// Tên chung của đối tượng.
    /// </summary>
    public string? Cn { get; init; }

    /// <summary>
    /// Danh sách các nhóm mà đối tượng là thành viên.
    /// </summary>
    public HashSet<string> MemberOf { get; init; } = [];

    /// <summary>
    /// Thời gian tạo đối tượng.
    /// </summary>
    public DateTimeOffset WhenCreated { get; init; }

    /// <summary>
    /// Thời gian thay đổi đối tượng lần cuối.
    /// </summary>
    public DateTimeOffset? WhenChanged { get; init; }
}