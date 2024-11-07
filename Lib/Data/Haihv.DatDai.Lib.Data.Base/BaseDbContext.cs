using Haihv.DatDai.Lib.Service.Audit.ToMongoDb;
using Haihv.DatDai.Lib.Service.Logger.MongoDb;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Haihv.DatDai.Lib.Data.Base;

public abstract class BaseDbContext(DbContextOptions options, IMongoDbContext mongoDbContext, IMemoryCache memoryCache) : DbContext(options)
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(new AuditInterceptor(mongoDbContext, memoryCache));
    }
}