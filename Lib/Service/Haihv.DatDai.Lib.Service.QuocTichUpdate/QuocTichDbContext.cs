using Haihv.DatDai.Lib.Data.Base;
using Haihv.DatDai.Lib.Data.DanhMuc.Entries;
using Haihv.DatDai.Lib.Service.Logger.MongoDb;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Haihv.DatDai.Lib.Service.QuocTichUpdate;

public class QuocTichDbContext : BaseDbContext
{
    public DbSet<QuocTich> QuocTich { get; init; }
    public QuocTichDbContext(
        INpgsqlDataConnectionService npgsqlDataConnectionService,
        IMongoDbContext mongoDbContext) : base(npgsqlDataConnectionService, mongoDbContext)
    {
    }
    
    public QuocTichDbContext(
        DbContextOptions<QuocTichDbContext> options,
        IMongoDbContext mongoDbContext) : base(options, mongoDbContext)
    {
    }
    public QuocTichDbContext(
        string connectionString,
        IMongoDbContext mongoDbContext) : base(connectionString, mongoDbContext)
    {
    }
}

public class ReadQuocTichDbContext(
    INpgsqlDataConnectionService npgsqlDataConnectionService,
    IMongoDbContext mongoDbContext) : QuocTichDbContext(npgsqlDataConnectionService.GetConnectionStringAsync(false).Result, mongoDbContext);