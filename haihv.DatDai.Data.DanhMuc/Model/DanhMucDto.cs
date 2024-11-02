using System.Text.Json.Serialization;

namespace haihv.DatDai.Data.DanhMuc.Model;

/// <summary>
/// Lớp cơ sở cho các đối tượng Danh Mục.
/// </summary>
public abstract class DanhMucDto
{
    /// <summary>
    /// ID của Danh Mục.
    /// </summary>
    [JsonPropertyName("id")]
    public Ulid Id { get; set; } = Ulid.NewUlid();
    
    /// <summary>
    /// Mã hoặc ký hiệu của Danh Mục.
    /// </summary>
    public virtual string MaKyHieu { get; set; } = string.Empty;   
    
    /// <summary>
    /// Tên hoặc giá trị của Danh Mục.
    /// </summary>
    public virtual string TenGiaTri { get; set; } = string.Empty;

    /// <summary>
    /// Ghi chú cho Danh Mục.
    /// </summary>
    [JsonPropertyName("ghiChu")]
    public string? GhiChu { get; set; }

    /// <summary>
    /// Thời gian tạo Danh Mục.
    /// </summary>
    [JsonPropertyName("createdAt")]
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;

    /// <summary>
    /// Thời gian cập nhật Danh Mục.
    /// </summary>
    [JsonPropertyName("updatedAt")]
    public DateTimeOffset? UpdatedAt { get; set; }
}