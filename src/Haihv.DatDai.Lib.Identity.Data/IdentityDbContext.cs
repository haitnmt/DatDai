using Audit.Core;
using Haihv.DatDai.Lib.Data.Base;
using Haihv.DatDai.Lib.Identity.Data.Entries;
using Microsoft.EntityFrameworkCore;

namespace Haihv.DatDai.Lib.Identity.Data;

public class IdentityDbContext(string connectionString, AuditDataProvider? auditDataProvider)
    : BaseDbContext(connectionString, auditDataProvider)
{
    public DbSet<User> Users { get; init; }
    public DbSet<Group> Groups { get; init; }
    //public DbSet<Role> Roles { get; init; }
    public DbSet<UserGroup> UserGroups { get; init; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .Property(u => u.RowVersion)
            .IsRowVersion();
        modelBuilder.Entity<Group>()
            .Property(g => g.RowVersion)
            .IsRowVersion();
        modelBuilder.Entity<UserGroup>()
            .Property(ug => ug.RowVersion)
            .IsRowVersion();
    }
}