using Haihv.DatDai.Lib.Data.DanhMuc.Model;
using Microsoft.EntityFrameworkCore;

namespace Haihv.DatDai.Lib.Service.QuocTichUpdate;

public class QuocTichDbContext(DbContextOptions<QuocTichDbContext> options) : DbContext(options)
{
    public DbSet<QuocTich> QuocTich { get; init; }
}