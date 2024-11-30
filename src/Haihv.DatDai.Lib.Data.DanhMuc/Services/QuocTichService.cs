using Elastic.Clients.Elasticsearch;
using Haihv.DatDai.Lib.Data.DanhMuc.Entries;
using Haihv.DatDai.Lib.Data.DanhMuc.Interfaces;
using Haihv.DatDai.Lib.Extension.Configuration;
using Microsoft.EntityFrameworkCore;

namespace Haihv.DatDai.Lib.Data.DanhMuc.Services;

public class QuocTichService(DanhMucDbContext danhMucDbContext, DanhMucDbContext danhMucDbContextReadOnly)  : IQuocTichService
{
    public QuocTichService(DanhMucDbContext danhMucDbContext) : this(danhMucDbContext, danhMucDbContext)
    {
    }

    public QuocTichService(PostgreSqlConnection postgreSqlConnection,
        ElasticsearchClientSettings elasticsearchClientSettings) : this(
        new DanhMucDbContext(postgreSqlConnection.PrimaryConnectionString, elasticsearchClientSettings),
        new DanhMucDbContext(postgreSqlConnection.ReplicaConnectionString, elasticsearchClientSettings))
    {
    }
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
                var existings = await danhMucDbContextReadOnly.QuocTich.Where(x =>
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
                danhMucDbContext.QuocTich.Add(quocTich);
                insert++;
            }

            await danhMucDbContext.SaveChangesAsync();
            index += bulkSize;
        }
        return (insert, update, skip);
    }
}