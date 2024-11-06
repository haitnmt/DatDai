using Microsoft.EntityFrameworkCore;

namespace Haihv.DatDai.Lib.Data.Base;

public abstract class BaseDbContext(DbContextOptions options) : DbContext(options)
{
    
}