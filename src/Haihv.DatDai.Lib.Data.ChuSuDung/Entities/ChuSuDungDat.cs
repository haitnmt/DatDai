using Haihv.DatDai.Lib.Data.ChuSuDung.Interfaces;

namespace Haihv.DatDai.Lib.Data.ChuSuDung.Entities;
/// <summary>
/// Lớp trừu tượng đại diện cho dữ liệu về người quản lý, sử dụng đất và chủ sở hữu tài sản gắn liền với đất.
/// </summary>
public abstract class ChuSuDungDat: ChuSuDungBase, IChuSuDungDat
{
    /// <summary>
    /// Người đại diện của chủ sử dụng đất.
    /// </summary>
    public virtual Guid? NguoiDaiDien { get; set; }
    
    /// <summary>
    /// Danh sách thành viên của chủ sử dụng đất.
    /// </summary>
    public virtual Guid[]? ThanhVien { get; set; }
    
}