using Haihv.DatDai.Lib.Data.DanhMuc.Entries;
using Microsoft.EntityFrameworkCore;

namespace Haihv.DatDai.Lib.Data.DanhMuc.Services;

public class DvhcService(DanhMucDbContext danhMucDbContext, ReadDanhMucDbContext? readDanhMucDbContext = default)
{
    private readonly DanhMucDbContext _readContext = readDanhMucDbContext ?? danhMucDbContext;
    public async Task<bool> DeleteByIdAsync(Guid id)
    {
      var result = await danhMucDbContext.Dvhc
          .Where(x => x.Id == id && !x.IsDeleted)
          .ExecuteUpdateAsync(x => 
              x.SetProperty(p =>p.IsDeleted, true)
              .SetProperty(p => p.DeletedAtUtc, DateTimeOffset.UtcNow));
      return result > 0;
    }

    public async Task<Dvhc?> GetByIdAsync(Guid id)
    {
        return await _readContext.Dvhc.FindAsync(id);
    }

    public async Task<IEnumerable<Dvhc>> GetAllAsync()
    {
        return await _readContext.Dvhc.ToListAsync();
    }
    
    public async Task<List<Dvhc>> GetDvhcByNameAsync(string name)
    {
        return await _readContext.Dvhc
            .Where(d => d.TenGiaTri.Contains(name))
            .ToListAsync();
    }
    public async Task UpdateDvhcAsync(Dvhc updatedDvhc)
    {
        var existingDvhc = await _readContext.Dvhc.FindAsync(updatedDvhc.Id);
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
            existingDvhc = await _readContext.Dvhc
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
            danhMucDbContext.Dvhc.Add(updatedDvhc);
        }
        await danhMucDbContext.SaveChangesAsync();
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
                var existing = await _readContext.Dvhc.FirstOrDefaultAsync(x => 
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
                danhMucDbContext.Dvhc.Add(dvhc);
                insert++;
            }

            await danhMucDbContext.SaveChangesAsync();
            index += bulkSize;
        }
        return (insert, update, skip);
    }
}