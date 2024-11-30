using Haihv.DatDai.Lib.Data.Base;

namespace Haihv.DatDai.Lib.Data.DanhMuc.Entries;

public interface IDanhMuc : IBaseDto
{
    string MaKyHieu { get; }
    string TenGiaTri { get; }
}