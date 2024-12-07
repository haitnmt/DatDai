using Haihv.DatDai.Lib.Data.DanhMuc.Entities;

namespace Haihv.DatDai.Lib.Data.DanhMuc.Interfaces;

public interface IQuocTichService
{
    Task<(int Insert, int Update, int Skip)> UpdateAsync(List<QuocTich> quocTiches, int bulkSize = 20);
}