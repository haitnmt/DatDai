using Haihv.DatDai.Data.Base;

namespace Haihv.DatDai.Data.DanhMuc.Model;

public interface IDanhMuc : IBaseDto
{
    string MaKyHieu { get; }
    string TenGiaTri { get; }
}