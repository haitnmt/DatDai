using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Haihv.DatDai.Lib.Data.ChuSuDung.Entities;

/// <summary>
/// Lớp đại diện cho hộ gia đình
/// </summary>
/// <remarks>
/// Chỉ áp dụng đối với dữ liệu trước ngày 01/8/2024
/// </remarks>
public class HoGiaDinh : ChuSuDungBase
{
    /// <summary>
    /// Thuộc tính ChuHo, kiểu dữ liệu UUID, là mã liên kết đến Cá Nhân,
    /// đại diện cho thông tin của chủ hộ.
    /// </summary>
    [Column("ChuHo", TypeName = "uuid")]
    [JsonPropertyName("chuHo")]
    public Guid ChuHo { get; set; } = Guid.Empty;
    /// <summary>
    /// Thuộc tính VoHoacChong, kiểu dữ liệu UUID, là mã liên kết đến Cá Nhân,
    /// đại diện cho thông tin của vợ hoặc chồng.
    /// </summary>
    [Column("VoHoacChong", TypeName = "uuid")]
    [JsonPropertyName("voHoacChong")]
    public Guid VoHoacChong { get; set; } = Guid.Empty;

    /// <summary>
    /// Danh sách thành viên của chủ sử dụng đất.
    /// Chứa dannh sách các thành viên trong hộ gia đình.
    /// </summary>
    [Column("ThanhVien", TypeName = "uuid[]")]
    [JsonPropertyName("thanhVien")]
    public virtual Guid[] ThanhVien { get; set; } = [];
}