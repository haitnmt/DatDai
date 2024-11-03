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

    public async Task UpdateDvhcAsync(List<Dvhc> dvhcs, int bulkSize = 1000)
    {
        var index = 0;
        while (index < dvhcs.Count)
        {
            var bulkDvhcs = dvhcs.Skip(index).Take(bulkSize).ToList();
            // Get existing DVHCs based on MaTinh, MaHuyen, MaXa Microsoft.EntityFrameworkCore.DbUpdateException: An error occurred while saving the entity changes. See the inner exception for details.
            //       ---> System.ArgumentException: Cannot write DateTime with Kind=Local to PostgreSQL type 'timestamp with time zone', only UTC is supported. Note that it's not possible to mix DateTimes with different Kinds in an array, range, or multirange. (Parameter 'value')


            foreach (var dvhc in bulkDvhcs)
            {
                var existing = await context.Dvhc.FirstOrDefaultAsync(x => 
                    x.MaTinh == dvhc.MaTinh && 
                    x.MaHuyen == dvhc.MaHuyen && 
                    x.MaXa == dvhc.MaXa);

                if (existing != null)
                {
                    if (existing.TenGiaTri == dvhc.TenGiaTri) continue;
                    existing.HieuLuc = false;
                    existing.UpdatedAt = DateTime.UtcNow;
                }

                context.Dvhc.Add(dvhc);
            }

            await context.SaveChangesAsync();
            index += bulkSize;
        }
    }

}