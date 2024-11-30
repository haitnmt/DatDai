using Audit.Core;
using Audit.Elasticsearch.Providers;
using Audit.EntityFramework;
using Elastic.Clients.Elasticsearch;
using Microsoft.EntityFrameworkCore;

namespace Haihv.DatDai.Lib.Data.Base;

public abstract class BaseDbContext : AuditDbContext
{
    private readonly string _connectionString;
    private readonly AuditDataProvider? _auditDataProvider;
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!string.IsNullOrEmpty(_connectionString) && !optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseNpgsql(_connectionString);
        }
        
        if (_auditDataProvider != null) 
            Audit.Core.Configuration.DataProvider = _auditDataProvider;
    }

    protected BaseDbContext(string connectionString, AuditDataProvider? auditDataProvider)
    {
        _connectionString = connectionString;
        _auditDataProvider = auditDataProvider;
    }

    protected BaseDbContext(string connectionString, ElasticsearchClientSettings? elasticsearchClientSettings)
    {
        _connectionString = connectionString;
        if (elasticsearchClientSettings is not null)
            _auditDataProvider = new ElasticsearchDataProvider
            {
                Settings = elasticsearchClientSettings,
                Index = new Setting<IndexName>(ev =>
                    $"{ev.GetEntityFrameworkEvent().Database.ToLower()}-audit-{DateTime.UtcNow:yyyy-MM}"),
                IdBuilder = _ => Guid.NewGuid()
            };
        else
            _auditDataProvider = null;
    }
}
