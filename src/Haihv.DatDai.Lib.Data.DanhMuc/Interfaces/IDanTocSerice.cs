using Haihv.DatDai.Lib.Data.DanhMuc.Entries;

namespace Haihv.DatDai.Lib.Data.DanhMuc.Interfaces;

public interface IDanTocSerice
{
    Task<List<DanToc>> GetAllDanTocAsync();
    Task<DanToc?> GetDanTocByIdAsync(Guid id);
    Task<DanToc?> GetDanTocByNameAsync(string name);
    Task UpdateDanTocAsync(DanToc updatedDanToc);
    Task<(int Insert, int Update, int Skip)> UpdateDanTocAsync(List<DanToc> danTocs);
    Task<(int Insert, int Update, int Skip)> UpdateDanTocAsync(string jsonFilePath);
    Task<(int Insert, int Update, int Skip)> UpdateDanTocAsync();
}