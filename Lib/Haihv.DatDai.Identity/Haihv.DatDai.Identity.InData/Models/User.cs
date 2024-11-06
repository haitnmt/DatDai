using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Haihv.DatDai.Identity.InData.Models;

/// <summary>
/// Lớp đại diện cho người dùng.
/// </summary>
public class User
{
    /// <summary>
    /// GUID của người dùng.
    /// </summary>
    [JsonPropertyName("id")]
    [Column("Id", TypeName = "varchar(36)")]
    [MaxLength(36)]
    public Guid Id { get; init; } = Guid.CreateVersion7();

    /// <summary>
    /// Mật khẩu của người dùng đã được mã hóa bằng BCrypt.Net.
    /// </summary>
    [Column("HashPassword", TypeName = "varchar(64)")]
    [JsonPropertyName("hashPassword")]
    [MaxLength(64)]
    public string? HashPassword { get; set; }

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
    [JsonPropertyName("authenticationType")]
    [Column("AuthenticationType", TypeName = "integer")]
    public int AuthenticationType { get; init; }

    /// <summary>
    /// Những vấn đề cần lưu ý của người dùng.
    /// </summary>
    [Column("GhiChu", TypeName = "varchar(50)")]
    [JsonPropertyName("ghiChu")]
    [MaxLength(50)]
    public string? GhiChu { get; set; }
}