using Haihv.DatDai.Lib.Service.Audit.ToMongoDb;
using Haihv.DatDai.Lib.Service.Logger.MongoDb;
using Microsoft.EntityFrameworkCore;

namespace Haihv.DatDai.Lib.Data.Base;

public abstract class BaseDbContext(DbContextOptions options, IMongoDbContext mongoDbContext) : DbContext(options)
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(new AuditInterceptor(mongoDbContext));
    }
}