using Microsoft.EntityFrameworkCore;

namespace Haihv.DatDai.Data.Identity;

public class IdentityDbContext(DbContextOptions<IdentityDbContext> options) : DbContext(options)
{
}