using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Haihv.DatDai.Lib.Data.ChuSuDung.Entries;

/// <summary>
/// Lớp đại diện cho cộng đồng dân cư, kế thừa từ ChuSuDungBase.
/// </summary>
/// <param name="tenCongDong">Tên của cộng đồng dân cư.</param>
public class CongDongDanCu(string tenCongDong) : ChuSuDungBase
{
    /// <summary>
    /// Thuộc tính TenCongDong, kiểu dữ liệu string, là tên của cộng đồng dân cư.
    /// </summary>
    [Column("TenCongDong", TypeName = "varchar(255)")]
    [JsonPropertyName("tenCongDong")]
    public string TenCongDong { get; set; } = tenCongDong;

    /// <summary>
    /// Thuộc tính NguoiDaiDien, kiểu dữ liệu UUID, là mã liên kết đến Cá Nhân,
    /// đại diện cho thông tin của người đại diện cộng đồng dân cư.
    /// </summary>
    [Column("NguoiDaiDien", TypeName = "uuid")]
    [JsonPropertyName("nguoiDaiDien")]
    public Guid NguoiDaiDien { get; set; } = Guid.Empty;

    /// <summary>
    /// Địa danh cư trú của cộng đồng dân cư.
    /// </summary>
    [Column("DiaDanhCuTru", TypeName = "varchar(255)")]
    [JsonPropertyName("diaDanhCuTru")]
    public string DiaDanhCuTru { get; set; } = string.Empty;
}
