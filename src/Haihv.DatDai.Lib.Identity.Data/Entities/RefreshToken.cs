using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Audit.EntityFramework;

namespace Haihv.DatDai.Lib.Identity.Data.Entities;

[AuditIgnore]
public class RefreshToken
{
    /// <summary>
    /// GUID của dòng dữ liệu.
    /// </summary>
    [Column("Id", TypeName = "uuid")]
    public Guid Id { get; init; } = Guid.CreateVersion7();
    /// <summary>
    /// GUID của người dùng.
    /// </summary>
    [Column("UserId", TypeName = "uuid")]
    public Guid UserId { get; init; } = Guid.Empty;
    /// <summary>
    /// Token của người dùng.
    /// </summary>
    [Column("Token", TypeName = "nvarchar(255)")]
    [MaxLength(255)]
    public string Token { get; init; } = string.Empty;

    /// <summary>
    /// Thời gian hết hạn của token.
    /// </summary>
    [Column("Expires", TypeName = "timestamp with time zone")]
    public DateTimeOffset Expires { get; init; } = DateTimeOffset.UtcNow.AddDays(7);
}