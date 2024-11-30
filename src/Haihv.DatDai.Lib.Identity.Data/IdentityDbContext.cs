using Microsoft.EntityFrameworkCore;

namespace Haihv.DatDai.Lib.Identity.Data;

public class IdentityDbContext(DbContextOptions<IdentityDbContext> options) : DbContext(options)
{
}