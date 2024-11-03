using haihv.DatDai.Data.DanhMuc.Model;
using Microsoft.EntityFrameworkCore;

namespace haihv.DatDai.Data.DanhMuc.Services;

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
            existingDvhc.UpdatedAt = DateTimeOffset.Now;
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
                    existingDvhc.UpdatedAt = DateTimeOffset.Now;
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

    public async Task UpdateDvhcAsync(List<Dvhc> dvhcs, int bulkSize = 1000)
    {
        var index = 0;
        while (index < dvhcs.Count)
        {
            var bulkDvhcs = dvhcs.Skip(index).Take(bulkSize).ToList();
            // Get existing DVHCs based on MaTinh, MaHuyen, MaXa
            var existingDvhcs = await context.Dvhc
                .Where(x => bulkDvhcs.Any(b => 
                    b.MaTinh == x.MaTinh && 
                    b.MaHuyen == x.MaHuyen && 
                    b.MaXa == x.MaXa))
                .ToListAsync();

            foreach (var dvhc in bulkDvhcs)
            {
                var existing = existingDvhcs.FirstOrDefault(x => 
                    x.MaTinh == dvhc.MaTinh && 
                    x.MaHuyen == dvhc.MaHuyen && 
                    x.MaXa == dvhc.MaXa);

                if (existing != null)
                {
                    if (existing.TenGiaTri == dvhc.TenGiaTri) continue;
                    existing.HieuLuc = false;
                    existing.UpdatedAt = DateTimeOffset.Now;
                }

                context.Dvhc.Add(dvhc);
            }

            await context.SaveChangesAsync();
            index += bulkSize;
        }
    }

}