using Haihv.DatDai.Lib.Data.Base;
using Haihv.DatDai.Lib.Data.DanhMuc.Entries;
using Haihv.DatDai.Lib.Service.Logger.MongoDb;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Haihv.DatDai.Lib.Service.QuocTichUpdate;

public class QuocTichDbContext(
    DbContextOptions<QuocTichDbContext> options,
    IMongoDbContext mongoDbContext, IMemoryCache memoryCache) : BaseDbContext(options, mongoDbContext, memoryCache)
{
    public DbSet<QuocTich> QuocTich { get; init; }
}