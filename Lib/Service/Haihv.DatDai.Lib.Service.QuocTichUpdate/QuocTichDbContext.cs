using Haihv.DatDai.Lib.Data.DanhMuc.Entries;
using Microsoft.EntityFrameworkCore;

namespace Haihv.DatDai.Lib.Service.QuocTichUpdate;

public class QuocTichDbContext(DbContextOptions<QuocTichDbContext> options) : DbContext(options)
{
    public DbSet<QuocTich> QuocTich { get; init; }
}