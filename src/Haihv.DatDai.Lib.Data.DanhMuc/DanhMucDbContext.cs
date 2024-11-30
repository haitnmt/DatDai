using Audit.Core;
using Elastic.Clients.Elasticsearch;
using Haihv.DatDai.Lib.Data.Base;
using Haihv.DatDai.Lib.Data.DanhMuc.Entries;
using Microsoft.EntityFrameworkCore;

namespace Haihv.DatDai.Lib.Data.DanhMuc;

public class DanhMucDbContext : BaseDbContext
{
    public DbSet<Dvhc> Dvhc { get; set; } = default!;
    public DbSet<DanToc> DanToc { get; set; } = default!;
    public DbSet<QuocTich> QuocTich { get; set; } = default!;
    public DanhMucDbContext(string connectionString, ElasticsearchClientSettings elasticsearchClientSettings) : base(connectionString, elasticsearchClientSettings)
    { }
    public DanhMucDbContext(string connectionString, AuditDataProvider auditDataProvider) : base(connectionString, auditDataProvider)
    { }
    
}