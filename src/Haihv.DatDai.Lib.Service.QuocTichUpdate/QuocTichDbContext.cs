using Audit.Core;
using Haihv.DatDai.Lib.Data.Base;
using Haihv.DatDai.Lib.Data.DanhMuc.Entries;
using Microsoft.EntityFrameworkCore;

namespace Haihv.DatDai.Lib.Service.QuocTichUpdate;

public class QuocTichDbContext(string connectionString, AuditDataProvider? auditDataProvider)
    : BaseDbContext(connectionString, auditDataProvider)
{
    public DbSet<QuocTich> QuocTich { get; set; } = default!;
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<QuocTich>()
            .HasQueryFilter(e => e.IsDeleted == false);
    }
}
