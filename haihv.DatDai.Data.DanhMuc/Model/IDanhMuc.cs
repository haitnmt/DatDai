using haihv.DatDai.Data.Base;

namespace haihv.DatDai.Data.DanhMuc.Model;

public interface IDanhMuc : IBaseDto
{
    string MaKyHieu { get; }
    string TenGiaTri { get; }
}