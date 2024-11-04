using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Haihv.DatDai.Data.Base;

public abstract class BaseDto
{
    /// <summary>
    /// ID của đối tượng.
    /// </summary>
    [JsonPropertyName("id")]
    public Guid Id { get; set; } = Guid.CreateVersion7();
    
    /// <summary>
    /// Ghi chú cho đối tượng.
    /// </summary>
    [JsonPropertyName("ghiChu")]
    [MaxLength(5000)]
    public string? GhiChu { get; set; }

    /// <summary>
    /// Thời gian tạo đối tượng.
    /// </summary>
    [JsonPropertyName("createdAt")]
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Thời gian cập nhật đối tượng.
    /// </summary>
    [JsonPropertyName("updatedAt")]
    public DateTimeOffset? UpdatedAt { get; set; }
}