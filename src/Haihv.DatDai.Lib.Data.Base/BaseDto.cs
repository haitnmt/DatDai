using System.ComponentModel.DataAnnotations.Schema;
using Audit.EntityFramework;

namespace Haihv.DatDai.Lib.Data.Base;

public abstract class BaseDto : IBaseDto
{
    /// <summary>
    /// Id của đối tượng.
    /// </summary>
    [Column("Id", TypeName = "uuid")]
    public Guid Id { get; init; } = Guid.CreateVersion7();

    /// <summary>
    /// Ghi chú cho đối tượng.
    /// </summary>
    [Column("GhiChu",TypeName = "text")]
    public virtual string? GhiChu { get; set; }

    /// <summary>
    /// Thời gian tạo đối tượng.
    /// </summary>
    [Column("CreatedAt", TypeName = "timestamp with time zone")]
    [AuditIgnore]
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Thời gian cập nhật đối tượng.
    /// </summary>
    [Column("UpdatedAt", TypeName = "timestamp with time zone")]
    [AuditIgnore]
    public DateTimeOffset? UpdatedAt { get; set; }

    /// <summary>
    /// Đánh dấu xóa mềm đối tượng.
    /// </summary>
    [Column("IsDeleted", TypeName = "boolean")]
    [AuditIgnore]
    public bool IsDeleted { get; set; }
    
    /// <summary>
    /// Thời gian đối tượng bị xóa mềm (UTC).
    /// </summary>
    [Column("DeletedAtUtc", TypeName = "timestamp with time zone")]
    [AuditIgnore]
    public DateTimeOffset? DeletedAtUtc { get; set; }
}