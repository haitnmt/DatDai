using haihv.DatDai.Data.DanhMuc.Model;
using Microsoft.EntityFrameworkCore;

namespace haihv.DatDai.Data.DanhMuc.Services;

public class DanhMucDbContext(DbContextOptions<DanhMucDbContext> options) : DbContext(options)
{
    public DbSet<Dvhc> Dvhc { get; init; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Dvhc>().HasKey(d => d.Id);
    }
}