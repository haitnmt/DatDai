using Haihv.DatDai.Data.DanhMuc.Model;
using Microsoft.EntityFrameworkCore;

namespace Haihv.DatDai.Data.DanhMuc.Services;

public class DanhMucDbContext(DbContextOptions<DanhMucDbContext> options) : DbContext(options)
{
    public DbSet<Dvhc> Dvhc { get; init; }
    public DbSet<DanToc> DanToc { get; init; }
    public DbSet<QuocTich> QuocTich { get; init; }
}