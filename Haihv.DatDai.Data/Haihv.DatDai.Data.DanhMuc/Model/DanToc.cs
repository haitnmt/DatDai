using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Haihv.DatDai.Data.Base;

namespace Haihv.DatDai.Data.DanhMuc.Model;

/// <summary>
/// Lớp đại diện cho thông tin dân tộc.
/// </summary>
public class DanToc : BaseDto, IDanhMuc
{
    /// <summary>
    /// Là mã của đơn vị hành chính trong dữ liệu
    /// </summary>
    [Column("Id",TypeName = "integer")]
    [JsonPropertyName("id")]
    public int Id { get; init; }
    /// <summary>
    /// Mã ký hiệu của dân tộc.
    /// </summary>
    [JsonPropertyName("maDanToc")]
    public string MaKyHieu => Id.ToString("00");

    /// <summary>
    /// Tên giá trị của dân tộc.
    /// </summary>
    [Column("TenDanToc",TypeName = "varchar(50)")]
    [JsonPropertyName("tenDanToc")]
    [MaxLength(50)]
    public string TenGiaTri { get; set; } = "Không rõ";

    /// <summary>
    /// Tên gọi khác của dân tộc.
    /// </summary>
    [Column("TenGoiKhac",TypeName = "text")]
    [JsonPropertyName("tenGoiKhac")]
    public override string? GhiChu { get; set; }
}