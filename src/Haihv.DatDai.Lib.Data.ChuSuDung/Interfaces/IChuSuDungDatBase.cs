using Haihv.DatDai.Lib.Data.Base;
using Haihv.DatDai.Lib.Data.Base.Entities;

namespace Haihv.DatDai.Lib.Data.ChuSuDung.Interfaces;

/// <summary>
/// Giao diện đại diện dữ liệu về người quản lý, sử dụng đất và chủ sở hữu tài sản gắn liền với đất.
/// </summary>
public interface IChuSuDungDatBase : IBaseDto
{
    /// <summary>
    /// ID của địa chỉ.
    /// </summary>
    Guid DiaChiId { get; }

    /// <summary>
    /// Phiên bản của chủ sử dụng đất.
    /// </summary>
    int PhienBan { get; set; }

    /// <summary>
    /// Trạng thái hiệu lực của chủ sử dụng đất.
    /// </summary>
    bool HieuLuc { get; set; }
}