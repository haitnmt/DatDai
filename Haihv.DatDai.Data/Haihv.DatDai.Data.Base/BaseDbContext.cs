using Microsoft.EntityFrameworkCore;

namespace Haihv.DatDai.Data.Base;

public abstract class BaseDbContext(DbContextOptions options) : DbContext(options)
{
    
}