using System.ComponentModel.DataAnnotations.Schema;

namespace Haihv.DatDai.Lib.Data.Base;

public abstract class BaseDto : IBaseDto
{
    /// <summary>
    /// Ghi chú cho đối tượng.
    /// </summary>
    [Column("GhiChu",TypeName = "text")]
    public virtual string? GhiChu { get; set; }

    /// <summary>
    /// Thời gian tạo đối tượng.
    /// </summary>
    [Column("CreatedAt", TypeName = "timestamp with time zone")]
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Thời gian cập nhật đối tượng.
    /// </summary>
    [Column("UpdatedAt", TypeName = "timestamp with time zone")]
    public DateTimeOffset? UpdatedAt { get; set; }

    /// <summary>
    /// Đánh dấu xóa mềm đối tượng.
    /// </summary>
    [Column("IsDeleted", TypeName = "boolean")]
    public bool IsDeleted { get; set; }
    
    /// <summary>
    /// Thời gian đối tượng bị xóa mềm (UTC).
    /// </summary>
    [Column("DeletedAtUtc", TypeName = "timestamp with time zone")]
    public DateTimeOffset DeletedAtUtc { get; set; }
}