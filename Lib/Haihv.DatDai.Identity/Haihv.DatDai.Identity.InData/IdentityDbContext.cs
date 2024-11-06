using Microsoft.EntityFrameworkCore;

namespace Haihv.DatDai.Identity.InData;

public class IdentityDbContext(DbContextOptions<IdentityDbContext> options) : DbContext(options)
{
}