using Haihv.DatDai.Lib.Data.Base;

namespace Haihv.DatDai.Lib.Data.DanhMuc.Model;

public interface IDanhMuc : IBaseDto
{
    string MaKyHieu { get; }
    string TenGiaTri { get; }
}