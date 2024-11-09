using System.Text.Json;
using System.Text.Json.Serialization;
using Haihv.DatDai.Lib.Data.Base;
using Haihv.DatDai.Lib.Data.DanhMuc.Entries;
using Haihv.DatDai.Lib.Service.Logger.MongoDb;
using Microsoft.EntityFrameworkCore;

namespace Haihv.DatDai.Lib.Data.DanhMuc.Services;

public class DanTocSerice
{
    private readonly DanhMucDbContext _danhMucDbContext;
    private readonly DanhMucDbContext _readContext;

    public DanTocSerice(DanhMucDbContext danhMucDbContext, ReadDanhMucDbContext? readDanhMucDbContext = default)
    {
        _danhMucDbContext = danhMucDbContext;
        _readContext = readDanhMucDbContext ?? danhMucDbContext;
    }
    
    public DanTocSerice(INpgsqlDataConnectionService npgsqlDataConnectionService, IMongoDbContext mongoDbContext)
    {
        _danhMucDbContext = new DanhMucDbContext(npgsqlDataConnectionService, mongoDbContext);
        _readContext = new ReadDanhMucDbContext(npgsqlDataConnectionService, mongoDbContext);
    }
    public  async Task<List<DanToc>> GetAllDanTocAsync()
    {
        return await _readContext.DanToc.ToListAsync();
    }
    public async Task<DanToc?> GetDanTocByIdAsync(Guid id)
    {
        return await _readContext.DanToc.FindAsync(id);
    }
    public async Task<DanToc?> GetDanTocByNameAsync(string name)
    {
        return await _readContext.DanToc
            .Where(d => d.TenGiaTri == name)
            .FirstOrDefaultAsync();
    }
    public async Task UpdateDanTocAsync(DanToc updatedDanToc)
    {
        var existingDanToc = await _readContext.DanToc.FindAsync(updatedDanToc.Id);
        if (existingDanToc != null)
        {
            existingDanToc.TenGiaTri = updatedDanToc.TenGiaTri;
            existingDanToc.GhiChu = updatedDanToc.GhiChu;
            existingDanToc.UpdatedAt = DateTimeOffset.UtcNow;
        }
        else
        {
            _danhMucDbContext.DanToc.Add(updatedDanToc);
        }

        await _danhMucDbContext.SaveChangesAsync();
    }

    public async Task<(int Insert, int Update, int Skip)> UpdateDvhcAsync(List<DanToc> danTocs)
    {
        var insert = 0;
        var update = 0;
        var skip = 0;
        foreach (var item in danTocs)
        {
            var existingDanToc = await _readContext.DanToc.FindAsync(item.Id);
            if (existingDanToc != null)
            {
                if (existingDanToc.TenGiaTri == item.TenGiaTri && existingDanToc.GhiChu == item.GhiChu)
                {
                    skip++;
                    continue;
                }
                existingDanToc.TenGiaTri = item.TenGiaTri;
                existingDanToc.GhiChu = item.GhiChu;
                existingDanToc.UpdatedAt = DateTimeOffset.UtcNow;
                update++;
            }
            else
            {
                _danhMucDbContext.DanToc.Add(item);
                insert++;
            }
        }
        await _danhMucDbContext.SaveChangesAsync();
        return (insert, update, skip);
    }

    public async Task<(int Insert, int Update, int Skip)> UpdateDvhcAsync(string jsonFilePath)
    {
        var jsonString = await File.ReadAllTextAsync(jsonFilePath);
        var jsonData = JsonSerializer.Deserialize<List<JsonModel>>(jsonString);
        if (jsonData != null)
        {
            return await UpdateDvhcAsync(ConvertFromJsonModel(jsonData));
        }
        return (0, 0, 0);
    }

    private static List<DanToc> ConvertFromJsonModel(List<JsonModel> jsonModels)
    {
        return jsonModels.Select(jm => new DanToc
        {
            Id = jm.MaDanToc,
            TenGiaTri = jm.TenDanToc,
            GhiChu = string.Join(", ", jm.TenGoiKhac)
        }).ToList();
    }
    private class JsonModel
    {
        [JsonPropertyName("maDanToc")]
        public int MaDanToc { get; init; }
        [JsonPropertyName("tenDanToc")] public string TenDanToc { get; init; } = string.Empty;
        [JsonPropertyName("tenGoiKhac")] public string[] TenGoiKhac { get; init; } = [];
    }
    public async Task<(int Insert, int Update, int Skip)> UpdateDvhcAsync()
    {
        var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "DanToc.json");
        return await UpdateDvhcAsync(filePath);
    }
}