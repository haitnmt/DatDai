using Haihv.DatDai.Lib.Data.Base;
using Haihv.DatDai.Lib.Data.DanhMuc.Entries;
using Haihv.DatDai.Lib.Service.Logger.MongoDb;
using Microsoft.EntityFrameworkCore;

namespace Haihv.DatDai.Lib.Data.DanhMuc;

public class DanhMucDbContext : BaseDbContext
{
    public DbSet<Dvhc> Dvhc { get; init; }
    public DbSet<DanToc> DanToc { get; init; }
    public DbSet<QuocTich> QuocTich { get; init; }
    
    public DanhMucDbContext(
        DbContextOptions<DanhMucDbContext> options,
        IMongoDbContext mongoDbContext) : base(options, mongoDbContext)
    {
    }
    public DanhMucDbContext(string connectionString,
        IMongoDbContext mongoDbContext) : base(connectionString,
        mongoDbContext)
    {
    }
    
    public DanhMucDbContext(INpgsqlDataConnectionService dataConnectionService,
        IMongoDbContext mongoDbContext) : base(dataConnectionService, mongoDbContext)
    {
    }
    
}

public class ReadDanhMucDbContext : DanhMucDbContext
{
    public ReadDanhMucDbContext(
        DbContextOptions<DanhMucDbContext> options,
        IMongoDbContext mongoDbContext) : base(options, mongoDbContext)
    {
    }

    public ReadDanhMucDbContext(string connectionString, IMongoDbContext mongoDbContext
    ) : base(connectionString, mongoDbContext)
    {
    }

    public ReadDanhMucDbContext(INpgsqlDataConnectionService dataConnectionService,
        IMongoDbContext mongoDbContext) : base(dataConnectionService.GetConnectionStringAsync(false).Result,
        mongoDbContext)
    {
    }
}