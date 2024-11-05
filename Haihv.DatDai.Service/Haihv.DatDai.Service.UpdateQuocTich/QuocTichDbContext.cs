using Haihv.DatDai.Data.DanhMuc.Model;
using Microsoft.EntityFrameworkCore;

namespace Haihv.DatDai.Service.UpdateQuocTich;

public class QuocTichDbContext(DbContextOptions<QuocTichDbContext> options) : DbContext(options)
{
    public DbSet<QuocTich> QuocTich { get; init; }
}