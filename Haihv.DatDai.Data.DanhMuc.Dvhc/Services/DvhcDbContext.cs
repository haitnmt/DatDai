using Haihv.DatDai.Data.Base;
using Haihv.DatDai.Data.DanhMuc.Services;
using Microsoft.EntityFrameworkCore;

namespace Haihv.DatDai.Data.DanhMuc.Dvhc.Services;

public class DvhcDbContext(DbContextOptions<DvhcDbContext> options) : DanhMucDbContext(options)
{
    public DbSet<Model.Dvhc> Dvhc { get; init; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Model.Dvhc>().HasKey(d => d.Id);
    }
}