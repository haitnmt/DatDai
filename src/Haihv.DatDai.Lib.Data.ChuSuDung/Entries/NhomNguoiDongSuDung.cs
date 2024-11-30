namespace Haihv.DatDai.Lib.Data.ChuSuDung.Entries;

/// <summary>
/// Lớp đại diện cho nhóm người đồng sử dụng.
/// Kế thừa từ lớp ChuSuDungDat.
/// </summary>
public class NhomNguoiDongSuDung : ChuSuDungBase
{
    /// <summary>
    /// Thuộc tính NguoiDaiDien, kiểu dữ liệu UUID, là mã liên kết đến Cá Nhân,
    /// đại diện cho thông tin của người đại diện nhóm người đồng sử dụng.
    /// </summary>
    public Guid NguoiDaiDien { get; set; } = Guid.Empty;

    /// <summary>
    /// Thuộc tính ThanhVien, kiểu dữ liệu mảng UUID, là danh sách mã liên kết đến Cá Nhân,
    /// đại diện cho thông tin của thành viên trong nhóm người đồng sử dụng.
    /// </summary>
    public Guid[] ThanhVien { get; set; } = [];
}
