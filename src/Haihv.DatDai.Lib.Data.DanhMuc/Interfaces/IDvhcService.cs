using Haihv.DatDai.Lib.Data.Base;
using Haihv.DatDai.Lib.Data.Base.Entities;
using Haihv.DatDai.Lib.Data.DanhMuc.Entities;

namespace Haihv.DatDai.Lib.Data.DanhMuc.Interfaces;

public interface IDvhcService : IBaseService<Dvhc>
{
    Task<List<Dvhc>> GetDvhcByNameAsync(string name);
    Task UpdateDvhcAsync(Dvhc updatedDvhc);
    Task<(int Insert, int Update, int Skip)> UpdateDvhcAsync(List<Dvhc> dvhcs, int bulkSize = 1000);
}