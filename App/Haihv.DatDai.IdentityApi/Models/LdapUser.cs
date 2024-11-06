using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Haihv.DatDai.IdentityApi.Interface;

namespace Haihv.DatDai.IdentityApi.Models;

/// <summary>
/// Lớp đại diện cho người dùng LDAP.
/// </summary>
public class LdapUser : ILdapUser
{
    /// <summary>
    /// GUID của người dùng.
    /// </summary>
    [JsonPropertyName("id")]
    [Column("Id", TypeName = "varchar(36)")]
    [MaxLength(36)]
    public virtual Guid Id { get; init; }

    /// <summary>
    /// Tên phân biệt của người dùng.
    /// </summary>
    [JsonPropertyName("distinguishedName")]
    [Column("DistinguishedName", TypeName = "varchar(255)")]
    [MaxLength(255)]
    public string? DistinguishedName { get; init; }

    /// <summary>
    /// Tên tài khoản SAM của người dùng.
    /// </summary>
    [JsonPropertyName("samAccountName")]
    [Column("SamAccountName", TypeName = "varchar(50)")]
    [MaxLength(50)]
    public string? SamAccountName { get; init; }

    /// <summary>
    /// Tên chung của người dùng.
    /// </summary>
    [JsonPropertyName("cn")]
    [Column("Cn", TypeName = "varchar(50)")]
    [MaxLength(50)]
    public string? Cn { get; init; }

    /// <summary>
    /// Email của người dùng.
    /// </summary>
    [JsonPropertyName("email")]
    [Column("Email", TypeName = "varchar(50)")]
    [MaxLength(50)]
    public string? Email { get; init; }

    /// <summary>
    /// Tên hiển thị của người dùng.
    /// </summary>
    [JsonPropertyName("displayName")]
    [Column("DisplayName", TypeName = "varchar(50)")]
    [MaxLength(50)]
    public string? DisplayName { get; init; }

    /// <summary>
    /// Tên chính của người dùng.
    /// </summary>
    [JsonPropertyName("givenName")]
    [Column("GivenName", TypeName = "varchar(50)")]
    [MaxLength(50)]
    public string? UserPrincipalName { get; init; }

    /// <summary>
    /// Chức danh công việc của người dùng.
    /// </summary>
    [JsonPropertyName("title")]
    [Column("JobTitle", TypeName = "varchar(50)")]
    [MaxLength(50)]
    public string? JobTitle { get; set; }

    /// <summary>
    /// Mô tả về người dùng.
    /// </summary>
    [JsonPropertyName("description")]
    [Column("Description", TypeName = "text")]
    public string? Description { get; init; }

    /// <summary>
    /// Phòng ban của người dùng.
    /// </summary>
    [JsonPropertyName("department")]
    [Column("Department", TypeName = "varchar(50)")]
    [MaxLength(50)]
    public string? Department { get; init; }

    /// <summary>
    /// Tổ chức của người dùng.
    /// </summary>
    [JsonPropertyName("organization")]
    [Column("Organization", TypeName = "varchar(150)")]
    [MaxLength(150)]
    public string? Organization { get; init; }

    /// <summary>
    /// URL miền của người dùng.
    /// </summary>
    [JsonPropertyName("domainUrl")]
    [Column("DomainUrl", TypeName = "varchar(100)")]
    [MaxLength(100)]
    public string? DomainUrl { get; init; }

    /// <summary>
    /// Trạng thái khóa của người dùng.
    /// </summary>
    [JsonPropertyName("isLocked")]
    [Column("IsLocked", TypeName = "boolean")]
    public bool IsLocked { get; init; }

    /// <summary>
    /// Trạng thái yêu cầu thay đổi mật khẩu của người dùng.
    /// </summary>
    [JsonPropertyName("isPwdMustChange")]
    [Column("IsPwdMustChange", TypeName = "boolean")]
    public bool IsPwdMustChange { get; init; }

    /// <summary>
    /// Thời gian mật khẩu được đặt lần cuối.
    /// </summary>
    [JsonPropertyName("pwdLastSet")]
    [Column("PwdLastSet", TypeName = "timestamp")]
    public DateTime PwdLastSet { get; init; }

    /// <summary>
    /// Các nhóm mà người dùng là thành viên.
    /// </summary>
    [JsonPropertyName("memberOf")]
    [Column("MemberOf", TypeName = "varchar(100)")]
    [MaxLength(100)]
    public HashSet<string> MemberOf { get; init; } = [];

    /// <summary>
    /// Thời gian tạo người dùng.
    /// </summary>
    [JsonPropertyName("whenCreated")]
    [Column("WhenCreated", TypeName = "timestamp with time zone")]

    public DateTimeOffset WhenCreated { get; init; } = DateTimeOffset.MinValue;

    /// <summary>
    /// Thời gian thay đổi người dùng lần cuối.
    /// </summary>
    [JsonPropertyName("whenChanged")]
    [Column("WhenChanged", TypeName = "timestamp with time zone")]
    public DateTimeOffset? WhenChanged { get; init; } = DateTimeOffset.UtcNow;
}