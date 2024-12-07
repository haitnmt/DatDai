using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Haihv.DatDai.Lib.Data.Base;
using Haihv.DatDai.Lib.Data.Base.Entities;

namespace Haihv.DatDai.Lib.Data.DanhMuc.Entities;

/// <summary>
/// Lớp đại diện cho thông tin dân tộc.
/// </summary>
public class DanToc : BaseDto, IDanhMuc
{
    /// <summary>
    /// Mã ký hiệu của dân tộc.
    /// </summary>
    [JsonPropertyName("maDanToc")]
    public string MaKyHieu => MaDanToc.ToString();

    /// <summary>
    /// Mã ký hiệu của dân tộc lưu trong cơ sở dữ liệu.
    /// </summary>
    [Column("MaDanToc",TypeName = "integer")]
    [JsonPropertyName("maDanToc")]
    public int MaDanToc { get; init; }

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