using Haihv.DatDai.Lib.Data.Base;
using Haihv.DatDai.Lib.Data.DanhMuc.Entries;
using Haihv.DatDai.Lib.Service.Logger.MongoDb;
using Microsoft.EntityFrameworkCore;

namespace Haihv.DatDai.Lib.Service.QuocTichUpdate;

public class QuocTichDbContext(
    DbContextOptions<QuocTichDbContext> options,
    IMongoDbContext mongoDbContext) : BaseDbContext(options, mongoDbContext)
{
    public DbSet<QuocTich> QuocTich { get; init; }
}