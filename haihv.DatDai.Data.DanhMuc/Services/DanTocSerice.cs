using System.Text.Json;
using System.Text.Json.Serialization;
using Haihv.DatDai.Data.DanhMuc.Model;
using Microsoft.EntityFrameworkCore;

namespace Haihv.DatDai.Data.DanhMuc.Services;

public class DanTocSerice(DanhMucDbContext context)
{
    public  async Task<List<DanToc>> GetAllDanTocAsync()
    {
        return await context.DanToc.ToListAsync();
    }
    public async Task<DanToc?> GetDanTocByIdAsync(Guid id)
    {
        return await context.DanToc.FindAsync(id);
    }
    public async Task<DanToc?> GetDanTocByNameAsync(string name)
    {
        return await context.DanToc
            .Where(d => d.TenGiaTri == name)
            .FirstOrDefaultAsync();
    }
    public async Task UpdateDanTocAsync(DanToc updatedDanToc)
    {
        var existingDanToc = await context.DanToc
            .Where(dt => dt.Id == updatedDanToc.Id || dt.MaKyHieu == updatedDanToc.MaKyHieu)
            .FirstOrDefaultAsync();
        if (existingDanToc != null)
        {
            existingDanToc.TenGiaTri = updatedDanToc.TenGiaTri;
            existingDanToc.MaKyHieu = updatedDanToc.MaKyHieu;
            existingDanToc.TenGoiKhac = updatedDanToc.TenGoiKhac;
            existingDanToc.UpdatedAt = DateTimeOffset.UtcNow;
        }
        else
        {
            context.DanToc.Add(updatedDanToc);
        }

        await context.SaveChangesAsync();
    }

    public async Task<(int Insert, int Update, int Skip)> UpdateDvhcAsync(List<DanToc> danTocs)
    {
        var insert = 0;
        var update = 0;
        var skip = 0;
        foreach (var item in danTocs)
        {
            var existingDanToc = await context.DanToc
                .Where(dt => dt.Id == item.Id || dt.MaKyHieu == item.MaKyHieu)
                .FirstOrDefaultAsync();
            if (existingDanToc != null)
            {
                if (existingDanToc.TenGiaTri == item.TenGiaTri && existingDanToc.TenGoiKhac.SequenceEqual(item.TenGoiKhac))
                {
                    skip++;
                    continue;
                }
                existingDanToc.TenGiaTri = item.TenGiaTri;
                existingDanToc.TenGoiKhac = item.TenGoiKhac;
                existingDanToc.UpdatedAt = DateTimeOffset.UtcNow;
                update++;
            }
            else
            {
                context.DanToc.Add(item);
                insert++;
            }
        }
        await context.SaveChangesAsync();
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
            MaKyHieu = jm.MaDanToc.ToString("00"),
            TenGiaTri = jm.TenDanToc,
            TenGoiKhac = jm.TenGoiKhac
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