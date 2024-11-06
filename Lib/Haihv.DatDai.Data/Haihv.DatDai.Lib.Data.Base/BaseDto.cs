using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Haihv.DatDai.Lib.Data.Base;

public abstract class BaseDto
{
    /// <summary>
    /// Ghi chú cho đối tượng.
    /// </summary>
    [JsonPropertyName("ghiChu")]
    [Column("GhiChu",TypeName = "text")]
    public virtual string? GhiChu { get; set; }

    /// <summary>
    /// Thời gian tạo đối tượng.
    /// </summary>
    [Column("CreatedAt", TypeName = "timestamp with time zone")]
    [JsonPropertyName("createdAt")]
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Thời gian cập nhật đối tượng.
    /// </summary>
    [Column("UpdatedAt", TypeName = "timestamp with time zone")]
    [JsonPropertyName("updatedAt")]
    public DateTimeOffset? UpdatedAt { get; set; }
}