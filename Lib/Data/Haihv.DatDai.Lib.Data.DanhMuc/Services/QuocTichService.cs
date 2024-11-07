using Haihv.DatDai.Lib.Data.DanhMuc.Entries;
using Microsoft.EntityFrameworkCore;

namespace Haihv.DatDai.Lib.Data.DanhMuc.Services;

public class QuocTichService(DanhMucDbContext context)
{
    public async Task<(int Insert, int Update, int Skip)> UpdateAsync(List<QuocTich> quocTiches, int bulkSize = 20)
    {
        var index = 0;
        var insert = 0;
        var update = 0;
        var skip = 0;
        while (index < quocTiches.Count)
        {
            var bulkDvhcs = quocTiches.Skip(index).Take(bulkSize).ToList();

            foreach (var quocTich in bulkDvhcs)
            {
                var existings = await context.QuocTich.Where(x =>
                    x.Id == quocTich.Id ||
                    x.Ccn3 == quocTich.Ccn3 ||
                    x.Cca3 == quocTich.Cca3).ToListAsync();
                if (existings.Count > 0)
                {
                    foreach (var existing in existings)
                    {
                        if (existing.TenQuocTe == quocTich.TenQuocTe &&
                            existing.TenQuocTeDayDu == quocTich.TenQuocTeDayDu &&
                            existing.Ccn3 == quocTich.Ccn3 &&
                            existing.Cca3 == quocTich.Cca3)
                        {
                            skip++;
                            continue;
                        }
                        existing.TenQuocGia = quocTich.TenQuocGia;
                        existing.TenDayDu = quocTich.TenDayDu;
                        existing.TenQuocTe = quocTich.TenQuocTe;
                        existing.TenQuocTeDayDu = quocTich.TenQuocTeDayDu;
                        existing.UpdatedAt = DateTimeOffset.UtcNow;
                        update++;
                    }
                }
                context.QuocTich.Add(quocTich);
                insert++;
            }

            await context.SaveChangesAsync();
            index += bulkSize;
        }
        return (insert, update, skip);
    }
}