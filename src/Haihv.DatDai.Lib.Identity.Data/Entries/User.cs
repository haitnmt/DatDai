using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Audit.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace Haihv.DatDai.Lib.Identity.Data.Entries;

/// <summary>
/// Lớp đại diện cho người dùng.
/// </summary>
[PrimaryKey("Id")]
public class User : BaseFromLdap
{
    /// <summary>
    /// Tên người dùng.
    /// </summary>
    [Column("UserName", TypeName = "varchar(50)")]
    [MaxLength(50)]
    [JsonPropertyName("username")]
    public string UserName { get; set; } = string.Empty;
    /// <summary>
    /// Email của người dùng.
    /// </summary>
    [Column("Email", TypeName = "varchar(50)")]
    [MaxLength(50)]
    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Tên hiển thị của người dùng.
    /// </summary>
    [Column("DisplayName", TypeName = "varchar(88)")]
    [MaxLength(88)]
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// Chức danh công việc của người dùng.
    /// </summary>
    [Column("JobTitle", TypeName = "varchar(150)")]
    [MaxLength(150)]
    public string? JobTitle { get; set; }
    
    /// <summary>
    /// Phòng ban của người dùng.
    /// </summary>
    [Column("Department", TypeName = "varchar(150)")]
    [MaxLength(150)]
    public string? Department { get; set; }

    /// <summary>
    /// Tổ chức của người dùng.
    /// </summary>
    [Column("Organization", TypeName = "varchar(150)")]
    [MaxLength(150)]
    public string? Organization { get; set; }

    /// <summary>
    /// URL miền của người dùng.
    /// </summary>
    [Column("DomainUrl", TypeName = "varchar(150)")]
    [MaxLength(150)]
    public string? DomainUrl { get; init; }

    /// <summary>
    /// Trạng thái khóa của người dùng.
    /// </summary>
    [Column("IsLocked", TypeName = "boolean")]
    public bool IsLocked { get; set; }

    /// <summary>
    /// Trạng thái yêu cầu thay đổi mật khẩu của người dùng.
    /// </summary>
    [Column("IsPwdMustChange", TypeName = "boolean")]
    public bool IsPwdMustChange { get; set; }

    /// <summary>
    /// Thời gian mật khẩu được đặt lần cuối.
    /// </summary>
    [Column("PwdLastSet", TypeName = "timestamp with time zone")]
    public DateTimeOffset PwdLastSet { get; set; }

    /// <summary>
    /// Mật khẩu của người dùng đã được mã hóa bằng BCrypt.Net.
    /// </summary>
    [Column("HashPassword", TypeName = "varchar(64)")]
    [MaxLength(64)]
    [AuditIgnore]
    public string? HashPassword { get; set; }

    /// <summary>
    /// Kiểu xác thực của người dùng.
    /// </summary>
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
    [JsonPropertyName("authenticationType")]
    [Column("AuthenticationType", TypeName = "integer")]
    public int AuthenticationType { get; init; }
    
}