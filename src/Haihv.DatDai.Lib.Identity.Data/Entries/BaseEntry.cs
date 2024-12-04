using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Haihv.DatDai.Lib.Data.Base;

namespace Haihv.DatDai.Lib.Identity.Data.Entries;

public abstract class BaseEntry : SoftDeletable
{
    /// <summary>
    /// GUID của dòng dữ liệu.
    /// </summary>
    [Column("Id", TypeName = "uuid")]
    [JsonPropertyName("id")]
    public Guid Id { get; init; } = Guid.CreateVersion7();
    /// <summary>
    /// Những vẫn đề cần chú ý (Description) của đối tượng
    /// </summary>
    [JsonPropertyName("ghiChu")]
    [Column("GhiChu", TypeName = "varchar(250)")]
    [MaxLength(250)]
    public string? GhiChu { get; set; }
    /// <summary>
    /// Thời gian tạo đối tượng.
    /// </summary>
    [JsonPropertyName("createdAt")]
    [Column("CreatedAt", TypeName = "timestamp with time zone")]
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Thời gian thay đổi lần cuối cùng của đối tượng.
    /// </summary>
    [JsonPropertyName("updatedAt")]
    [Column("UpdatedAt", TypeName = "timestamp with time zone")]
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
    /// <summary>
    /// Phiên bản của dòng dữ liệu.
    /// </summary>
    [JsonPropertyName("rowVersion")]
    [Timestamp]
    public uint RowVersion { get; set; }
}