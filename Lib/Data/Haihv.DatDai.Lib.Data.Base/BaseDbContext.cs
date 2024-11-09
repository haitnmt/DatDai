using Haihv.DatDai.Lib.Service.Audit.ToMongoDb;
using Haihv.DatDai.Lib.Service.Logger.MongoDb;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Haihv.DatDai.Lib.Data.Base;

public abstract class BaseDbContext: DbContext
{
    private readonly IMongoDbContext _mongoDbContext;
    private readonly IMemoryCache _memoryCache = new MemoryCache(new MemoryCacheOptions());
    private readonly string _connectionString;
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured && !string.IsNullOrEmpty(_connectionString))
        {
            optionsBuilder.UseNpgsql(_connectionString);
        }
        optionsBuilder.AddInterceptors(new AuditInterceptor(_mongoDbContext, _memoryCache));
    }

    protected BaseDbContext(DbContextOptions options, IMongoDbContext mongoDbContext)
    {
        _mongoDbContext = mongoDbContext;
        _connectionString = string.Empty;
    }

    protected BaseDbContext(string connnectionString, IMongoDbContext mongoDbContext)
    {
        _mongoDbContext = mongoDbContext;
        _connectionString = connnectionString;
    }

    protected BaseDbContext(INpgsqlDataConnectionService dataConnectionService, IMongoDbContext mongoDbContext, bool isPrimary = true)
    {
        _mongoDbContext = mongoDbContext;
        _connectionString = dataConnectionService.GetConnectionStringAsync(isPrimary).Result;
    }
}
