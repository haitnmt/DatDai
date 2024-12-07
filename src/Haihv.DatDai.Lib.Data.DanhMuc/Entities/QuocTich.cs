using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Haihv.DatDai.Lib.Data.Base;
using Haihv.DatDai.Lib.Data.Base.Entities;

namespace Haihv.DatDai.Lib.Data.DanhMuc.Entities;

public class QuocTich : BaseDto, IDanhMuc
{
    /// <summary>
    /// Mã quốc gia dạng số
    /// </summary>
    /// <remarks>
    /// Theo tiêu chuẩn ISO 3166-1.
    /// </remarks>
    [Column("ccn3",TypeName = "integer")]
    [JsonPropertyName("ccn3")]
    public int Ccn3 { get; init; }
    /// <summary>
    /// Mã quốc gia dạng chữ 3 ký tự
    /// </summary>
    /// <remarks>
    /// Theo tiêu chuẩn ISO 3166-1.
    /// </remarks>
    [Column("cca3",TypeName = "varchar(3)")]
    [JsonPropertyName("cca3")]
    [MaxLength(3)]
    public string Cca3 { get; init; } = string.Empty;

    [JsonPropertyName("maQuocGia")]
    public string MaKyHieu  => Cca3;

    [JsonPropertyName("tenQuocGiaTV")] 
    public string TenGiaTri => TenQuocGia ?? string.Empty;
    
    /// <summary>
    /// Tên quốc gia theo tiếng Việt.
    /// common name in Vietnamese
    /// </summary>
    [Column("TenQuocGia",TypeName = "varchar(50)")]
    [JsonPropertyName("tenQuocGia")]
    [MaxLength(50)]
    public string? TenQuocGia { get; set; } = string.Empty;
    
    /// <summary>
    /// Tên quốc gia đầy đủ theo tiếng Việt.
    /// official name in Vietnamese
    /// </summary>
    [Column("TenDayDu",TypeName = "varchar(50)")]
    [JsonPropertyName("tenDayDuTV")]
    [MaxLength(150)]
    public string? TenDayDu { get; set; } = string.Empty;

    /// <summary>
    /// Tên quốc gia theo quốc tế.
    /// common name
    /// </summary>
    [Column("TenQuocTe",TypeName = "varchar(50)")]
    [MaxLength(50)]
    [JsonPropertyName("tenQuocGiaQT")]
    public string TenQuocTe { get; set; } = string.Empty;
    
    /// <summary>
    /// Tên quốc gia đầy đủ theo quốc tế.
    /// official name
    /// </summary>
    [Column("TenQuocTeDayDu",TypeName = "varchar(150)")]
    [MaxLength(150)]
    [JsonPropertyName("tenDayDuQT")]
    public string TenQuocTeDayDu { get; set; } = string.Empty;
}