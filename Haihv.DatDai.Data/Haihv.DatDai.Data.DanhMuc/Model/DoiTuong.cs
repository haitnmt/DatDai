using System.Runtime.InteropServices.JavaScript;
using System.Text.Json.Serialization;
using Haihv.DatDai.Data.Base;

namespace Haihv.DatDai.Data.DanhMuc.Model;

/// <summary>
/// Lớp đại diện cho danh mục đối tượng sử dụng đất
/// </summary>
public class DoiTuong: BaseDto, IDanhMuc
{
    /// <summary>
    /// Mã đối tượng sử dụng đất.
    /// </summary>
    [JsonPropertyName("maDoiTuong")] public string MaKyHieu { get; init; } = string.Empty;

    /// <summary>
    /// Giá trị của đối tượng sử dụng đất.
    /// </summary>
    [JsonPropertyName("giaTri")] public string TenGiaTri { get; init; } = string.Empty;
}