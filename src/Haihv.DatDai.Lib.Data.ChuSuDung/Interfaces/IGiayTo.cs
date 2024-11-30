using Haihv.DatDai.Lib.Data.Base;

namespace Haihv.DatDai.Lib.Data.ChuSuDung.Interfaces;

public interface IGiayTo : IBaseDto
{
    /// <summary>
    /// Loại giấy tờ.
    /// </summary>
    int LoaiGiayTo { get; set; }
    /// <summary>
    /// Số giấy tờ.
    /// </summary>
    string SoGiayTo { get; set; }
    /// <summary>
    /// Ngày cấp giấy tờ.
    /// </summary>
    DateTime NgayCap { get; set; }
    /// <summary>
    /// Nơi cấp giấy tờ.
    /// </summary>
    string NoiCap { get; set; }
    /// <summary>
    /// Mã định danh của giấy tờ.
    /// </summary>
    string? MaDinhDanh { get; set; }
    /// <summary>
    /// Hình thức xác thực giấy tờ.
    /// </summary>
    /// <remarks>
    /// 1: là trạng thái xác thực qua giấy tờ tùy thân
    /// 2: là trạng thái đã xác thực qua VNID
    /// 3: là hình thức xác thực khác
    /// </remarks>
    int HinhThucXacThuc { get; set; }
    /// <summary>
    /// Phiên bản của giấy tờ.
    /// </summary>
    int PhienBan { get; set; }
}