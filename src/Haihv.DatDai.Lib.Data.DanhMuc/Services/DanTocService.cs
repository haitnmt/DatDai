using System.Text.Json;
using System.Text.Json.Serialization;
using Elastic.Clients.Elasticsearch;
using Haihv.DatDai.Lib.Data.DanhMuc.Entries;
using Haihv.DatDai.Lib.Data.DanhMuc.Interfaces;
using Haihv.DatDai.Lib.Extension.Configuration;
using Microsoft.EntityFrameworkCore;

namespace Haihv.DatDai.Lib.Data.DanhMuc.Services;

public class DanTocService(DanhMucDbContext danhMucDbContext, DanhMucDbContext danhMucDbContextReadOnly) : IDanTocSerice
{
    public DanTocService(DanhMucDbContext danhMucDbContext) : this(danhMucDbContext, danhMucDbContext)
    {
    }

    public DanTocService(PostgreSqlConnection postgreSqlConnection,
        ElasticsearchClientSettings elasticsearchClientSettings) : this(
        new DanhMucDbContext(postgreSqlConnection.PrimaryConnectionString, elasticsearchClientSettings),
        new DanhMucDbContext(postgreSqlConnection.ReplicaConnectionString, elasticsearchClientSettings))
    {
    }
    public  async Task<List<DanToc>> GetAllDanTocAsync()
    {
        return await danhMucDbContextReadOnly.DanToc.ToListAsync();
    }
    public async Task<DanToc?> GetDanTocByIdAsync(Guid id)
    {
        return await danhMucDbContextReadOnly.DanToc.FindAsync(id);
    }
    public async Task<DanToc?> GetDanTocByNameAsync(string name)
    {
        return await danhMucDbContextReadOnly.DanToc
            .Where(d => d.TenGiaTri == name)
            .FirstOrDefaultAsync();
    }
    public async Task UpdateDanTocAsync(DanToc updatedDanToc)
    {
        var existingDanToc = await danhMucDbContextReadOnly.DanToc.FindAsync(updatedDanToc.Id);
        if (existingDanToc != null)
        {
            existingDanToc.TenGiaTri = updatedDanToc.TenGiaTri;
            existingDanToc.GhiChu = updatedDanToc.GhiChu;
            existingDanToc.UpdatedAt = DateTimeOffset.UtcNow;
        }
        else
        {
            danhMucDbContext.DanToc.Add(updatedDanToc);
        }

        await danhMucDbContext.SaveChangesAsync();
    }

    public async Task<(int Insert, int Update, int Skip)> UpdateDanTocAsync(List<DanToc> danTocs)
    {
        var insert = 0;
        var update = 0;
        var skip = 0;
        foreach (var item in danTocs)
        {
            var existingDanToc = await danhMucDbContextReadOnly.DanToc.FindAsync(item.Id);
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
                danhMucDbContext.DanToc.Add(item);
                insert++;
            }
        }
        await danhMucDbContext.SaveChangesAsync();
        return (insert, update, skip);
    }

    public async Task<(int Insert, int Update, int Skip)> UpdateDanTocAsync(string jsonFilePath)
    {
        var jsonString = await File.ReadAllTextAsync(jsonFilePath);
        var jsonData = JsonSerializer.Deserialize<List<JsonModel>>(jsonString);
        if (jsonData != null)
        {
            return await UpdateDanTocAsync(ConvertFromJsonModel(jsonData));
        }
        return (0, 0, 0);
    }

    private static List<DanToc> ConvertFromJsonModel(List<JsonModel> jsonModels)
    {
        return jsonModels.Select(jm => new DanToc
        {
            MaDanToc = jm.MaDanToc,
            TenGiaTri = jm.TenDanToc,
            GhiChu = string.Join(", ", jm.TenGoiKhac)
        }).ToList();
    }
    private class JsonModel
    {
        [JsonPropertyName("maDanToc")] public int MaDanToc { get; init; }
        [JsonPropertyName("tenDanToc")] public string TenDanToc { get; init; } = string.Empty;
        [JsonPropertyName("tenGoiKhac")] public string[] TenGoiKhac { get; init; } = [];
    }
    public async Task<(int Insert, int Update, int Skip)> UpdateDanTocAsync()
    {
        var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "DanToc.json");
        return await UpdateDanTocAsync(filePath);
    }
}