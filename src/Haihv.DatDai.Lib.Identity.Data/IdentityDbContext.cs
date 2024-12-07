using Audit.Core;
using Haihv.DatDai.Lib.Data.Base;
using Haihv.DatDai.Lib.Data.Base.Extensions;
using Haihv.DatDai.Lib.Identity.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Haihv.DatDai.Lib.Identity.Data;

public class IdentityDbContext(string connectionString, AuditDataProvider? auditDataProvider)
    : BaseDbContext(connectionString, auditDataProvider)
{
    public DbSet<User> Users { get; init; } = default!;
    public DbSet<Group> Groups { get; init; } = default!;
    //public DbSet<Role> Roles { get; init; } = default!;
    public DbSet<UserGroup> UserGroups { get; init; } = default!;
    public DbSet<RefreshToken> RefreshTokens { get; init; } = default!;
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<User>()
            .Property(u => u.RowVersion)
            .IsRowVersion();
        modelBuilder.Entity<Group>()
            .Property(g => g.RowVersion)
            .IsRowVersion();
        modelBuilder.Entity<UserGroup>()
            .Property(ug => ug.RowVersion)
            .IsRowVersion();
        modelBuilder.ApplySoftDeleteQueryFilter();
    }
}