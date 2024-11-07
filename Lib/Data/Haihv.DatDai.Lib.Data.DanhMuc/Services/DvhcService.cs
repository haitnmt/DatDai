using Haihv.DatDai.Lib.Data.DanhMuc.Model;
using Microsoft.EntityFrameworkCore;

namespace Haihv.DatDai.Lib.Data.DanhMuc.Services;

public class DvhcService(DanhMucDbContext context)
{
    public async Task<List<Dvhc>> GetAllDvhcAsync()
    {
        return await context.Dvhc.ToListAsync();
    }

    public async Task<Dvhc?> GetDvhcByIdAsync(Guid id)
    {
        return await context.Dvhc.FindAsync(id);
    }

    public async Task<List<Dvhc>> GetDvhcByNameAsync(string name)
    {
        return await context.Dvhc
            .Where(d => d.TenGiaTri.Contains(name))
            .ToListAsync();
    }
    public async Task UpdateDvhcAsync(Dvhc updatedDvhc)
    {
        var existingDvhc = await context.Dvhc.FindAsync(updatedDvhc.Id);
        if (existingDvhc != null)
        {
            existingDvhc.TenGiaTri = updatedDvhc.TenGiaTri;
            existingDvhc.MaXa = updatedDvhc.MaXa;
            existingDvhc.MaHuyen = updatedDvhc.MaHuyen;
            existingDvhc.MaTinh = updatedDvhc.MaTinh;
            existingDvhc.Cap = updatedDvhc.Cap;
            existingDvhc.LoaiHinh = updatedDvhc.LoaiHinh;
            existingDvhc.NgayHieuLuc = updatedDvhc.NgayHieuLuc;
            existingDvhc.HieuLuc = updatedDvhc.HieuLuc;
            existingDvhc.UpdatedAt = DateTime.UtcNow;
        }
        else
        {
            existingDvhc = await context.Dvhc
                .Where(x => x.MaTinh == updatedDvhc.MaTinh && 
                                 x.MaHuyen == updatedDvhc.MaHuyen && 
                                 x.MaXa == updatedDvhc.MaXa).FirstOrDefaultAsync();
            if (existingDvhc != null)
            {
                if (existingDvhc.TenGiaTri != updatedDvhc.TenGiaTri)
                {
                    // Cập nhật lại thông tin cũ:
                    existingDvhc.HieuLuc = false;
                    existingDvhc.UpdatedAt = DateTime.UtcNow;
                }
                else
                {
                    return;
                }
            }
            // Thêm mới:
            context.Dvhc.Add(updatedDvhc);
        }
        await context.SaveChangesAsync();
    }

    public async Task<(int Insert, int Update, int Skip)> UpdateDvhcAsync(List<Dvhc> dvhcs, int bulkSize = 1000)
    {
        var index = 0;
        var insert = 0;
        var update = 0;
        var skip = 0;
        while (index < dvhcs.Count)
        {
            var bulkDvhcs = dvhcs.Skip(index).Take(bulkSize).ToList();

            foreach (var dvhc in bulkDvhcs)
            {
                var existing = await context.Dvhc.FirstOrDefaultAsync(x => 
                    x.MaTinh == dvhc.MaTinh && 
                    x.MaHuyen == dvhc.MaHuyen && 
                    x.MaXa == dvhc.MaXa);

                if (existing != null)
                {
                    if (existing.TenGiaTri == dvhc.TenGiaTri)
                    {
                        skip++;
                        continue;
                    }
                    existing.HieuLuc = false;
                    existing.UpdatedAt = DateTimeOffset.UtcNow;
                    update++;
                }
                context.Dvhc.Add(dvhc);
                insert++;
            }

            await context.SaveChangesAsync();
            index += bulkSize;
        }
        return (insert, update, skip);
    }

}