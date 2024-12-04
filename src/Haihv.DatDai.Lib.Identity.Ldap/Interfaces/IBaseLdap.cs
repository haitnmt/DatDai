namespace Haihv.DatDai.Lib.Identity.Ldap.Interfaces;

/// <summary>
/// Giao diện đại diện cho một đối tượng LDAP.
/// </summary>
public interface IBaseLdap
{
    /// <summary>
    /// Định danh duy nhất của đối tượng LDAP.
    /// </summary>
    Guid Id { get; init; }

    /// <summary>
    /// Tên phân biệt của đối tượng LDAP.
    /// </summary>
    string DistinguishedName { get; init; }

    /// <summary>
    /// Tên tài khoản SAM của đối tượng LDAP.
    /// </summary>
    string? SamAccountName { get; init; }

    /// <summary>
    /// Tên hiển thị của đối tượng LDAP.
    /// </summary>
    string? Cn { get; init; }
    /// <summary>
    /// Tập hợp các nhóm mà đối tượng LDAP là thành viên.
    /// </summary>
    HashSet<string> MemberOf { get; init; }

    /// <summary>
    /// Thời điểm tạo đối tượng LDAP.
    /// </summary>
    DateTimeOffset WhenCreated { get; init; }

    /// <summary>
    /// Thời điểm thay đổi đối tượng LDAP.
    /// </summary>
    DateTimeOffset? WhenChanged { get; init; }
}