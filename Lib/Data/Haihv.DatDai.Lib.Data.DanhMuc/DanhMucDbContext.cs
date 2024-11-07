using Haihv.DatDai.Lib.Data.Base;
using Haihv.DatDai.Lib.Data.DanhMuc.Entries;
using Haihv.DatDai.Lib.Service.Logger.MongoDb;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Haihv.DatDai.Lib.Data.DanhMuc;

public class DanhMucDbContext(DbContextOptions<DanhMucDbContext> options, IMongoDbContext mongoDbContext, IMemoryCache memoryCache) : BaseDbContext(options, mongoDbContext, memoryCache)
{
    public DbSet<Dvhc> Dvhc { get; init; }
    public DbSet<DanToc> DanToc { get; init; }
    public DbSet<QuocTich> QuocTich { get; init; }
    
}