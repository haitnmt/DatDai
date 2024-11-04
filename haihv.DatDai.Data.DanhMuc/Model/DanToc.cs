using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Haihv.DatDai.Data.Base;

namespace Haihv.DatDai.Data.DanhMuc.Model;

/// <summary>
/// Lớp đại diện cho thông tin dân tộc.
/// </summary>
public class DanToc : BaseDto, IDanhMuc
{
    /// <summary>
    /// Mã ký hiệu của dân tộc.
    /// </summary>
    [JsonPropertyName("maDanToc")]
    [MaxLength(2)]
    public string MaKyHieu { get; set; } = "56";

    /// <summary>
    /// Tên giá trị của dân tộc.
    /// </summary>
    [JsonPropertyName("tenDanToc")]
    [MaxLength(50)]
    public string TenGiaTri { get; set; } = "Không rõ";

    /// <summary>
    /// Tên gọi khác của dân tộc.
    /// </summary>
    [JsonPropertyName("tenGoiKhac")]
    [MaxLength(50)]
    public string[] TenGoiKhac { get; set; } = [];
}