using Haihv.DatDai.Lib.Data.Base;
using Haihv.DatDai.Lib.Data.Base.Entities;

namespace Haihv.DatDai.Lib.Data.DanhMuc.Entities;

public interface IDanhMuc : IBaseDto
{
    string MaKyHieu { get; }
    string TenGiaTri { get; }
}