using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Haihv.DatDai.Data.Base;

namespace Haihv.DatDai.Data.DanhMuc.Model;

public class QuocTich : BaseDto, IDanhMuc
{
    /// <summary>
    /// Mã quốc gia.
    /// </summary>
    /// <remarks>
    /// Theo tiêu chuẩn ISO 3166-1.
    /// </remarks>
    [JsonPropertyName("maQuocGia")]
    [MaxLength(5)]
    public string? MaKyHieu { get; set; }= string.Empty;
    /// <summary>
    /// Tên quốc gia theo tiếng Việt.
    /// </summary>
    [JsonPropertyName("tenQuocGiaTV")]
    [MaxLength(150)]
    public string? TenGiaTri { get; set; } = string.Empty;
    
    /// <summary>
    /// Tên quốc gia theo quốc tế.
    /// </summary>
    [MaxLength(150)]
    [JsonPropertyName("tenQuocGiaQT")]
    public string TenQuocGiaQt { get; set; } = string.Empty;
}