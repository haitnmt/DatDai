namespace Haihv.DatDai.Lib.Data.ChuSuDung.Interfaces;

/// <summary>
/// Giao diện đại diện dữ liệu về người quản lý, sử dụng đất và chủ sở hữu tài sản gắn liền với đất.
/// </summary>
public interface IChuSuDungDat : IChuSuDungDatBase
{
    /// <summary>
    /// ID của người đại diện.
    /// </summary>
    Guid? NguoiDaiDien { get; }

    /// <summary>
    /// Danh sách ID của các thành viên.
    /// </summary>
    Guid[]? ThanhVien { get; }
}