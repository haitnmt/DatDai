using Audit.Core;
using Audit.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace Haihv.DatDai.Lib.Data.Base;

public abstract class BaseDbContext(string connectionString, AuditDataProvider? auditDataProvider)
    : AuditDbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!string.IsNullOrEmpty(connectionString) && !optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseNpgsql(connectionString);
        }
        
        if (auditDataProvider != null) 
            Audit.Core.Configuration.DataProvider = auditDataProvider;
    }
}
