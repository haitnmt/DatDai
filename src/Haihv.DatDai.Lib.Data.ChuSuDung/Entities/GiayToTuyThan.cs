using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Haihv.DatDai.Lib.Data.ChuSuDung.Entities;

/// <summary>
/// Lớp đại diện cho giấy tờ tùy thân.
/// </summary>
/// <param name="soGiayTo">Số giấy tờ.</param>
/// <param name="loaiGiayToTuyThan">Loại giấy tờ tùy thân, mặc định là 0.</param>
public class GiayToTuyThan(string soGiayTo, int loaiGiayToTuyThan = 0) : GiayTo(soGiayTo)
{
    /// <summary>
    /// Loại giấy tờ tùy thân.
    /// <para>Quy định tại bảng mã loại giấy tờ. (1.16 - Thông tư 09/2020/TT-BTNMT)</para>
    /// <c>1: Giấy khai sinh</c>
    /// <c>2: Chứng minh nhân dân</c>
    /// <c>3: Giấy chứng minh sỹ quan quân đội nhân dân Việt Nam</c>
    /// <c>4: Giấy chứng minh công an nhân dân</c>
    /// <c>5: Căn cước công dân</c>
    /// <c>6: Hộ chiếu</c>
    /// <c>7: Sổ hộ khẩu</c>
    /// <c>8: Các loại giấy tờ tùy thân khác</c>
    /// <c>9: Thẻ căn cước</c>
    /// <c>10: Mã định danh cá nhân</c>
    /// </summary>
    /// <remarks>
    /// <para>Ghi chú:</para>
    /// Dữ liệu về Chứng minh nhân dân chỉ áp dụng với các dữ liệu hình thành trước ngày 01/01/2025,
    /// <para>Dữ liệu về Sổ hộ khẩu chỉ áp dụng với các dữ liệu hình thành trước ngày 01/8/2024.</para>
    /// </remarks>
    [Column("LoaiGiayToTuyThan",TypeName = "integer")]
    [JsonPropertyName("loaiGiayToTuyThan")]
    public override int LoaiGiayTo { get; set; } = loaiGiayToTuyThan;

    /// <summary>
    /// Mã định danh cá nhân.
    /// </summary>
    [Column("MaDinhDanhCaNhan",TypeName = "varchar(150)")]
    [JsonPropertyName("maDinhDanhCaNhan")]
    public override string? MaDinhDanh { get; set; }

    /// <summary>
    /// Phiên bản cá nhân.
    /// </summary>
    [Column("PhienBanCaNhan",TypeName = "integer")]
    [JsonPropertyName("phienBanCaNhan")]
    public override int PhienBan { get; set; }
}