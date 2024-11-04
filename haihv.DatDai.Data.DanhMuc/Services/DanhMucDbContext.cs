using Haihv.DatDai.Data.DanhMuc.Model;
using Microsoft.EntityFrameworkCore;

namespace Haihv.DatDai.Data.DanhMuc.Services;

public abstract class DanhMucDbContext(DbContextOptions options) : DbContext(options)
{
}