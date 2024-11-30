using Audit.Core;
using Elastic.Clients.Elasticsearch;
using Haihv.DatDai.Lib.Data.Base;
using Haihv.DatDai.Lib.Data.DanhMuc.Entries;
using Microsoft.EntityFrameworkCore;

namespace Haihv.DatDai.Lib.Service.QuocTichUpdate;

public class QuocTichDbContext : BaseDbContext
{
    public DbSet<QuocTich> QuocTich { get; set; } = default!;
    
    public QuocTichDbContext(string connectionString, ElasticsearchClientSettings elasticsearchClientSettings) : base(connectionString, elasticsearchClientSettings)
    { }
    public QuocTichDbContext(string connectionString, AuditDataProvider auditDataProvider) : base(connectionString, auditDataProvider)
    { }
}
