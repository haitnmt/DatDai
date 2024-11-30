using Audit.Core;
using Elastic.Clients.Elasticsearch;
using Haihv.DatDai.Lib.Data.Base;
using Haihv.DatDai.Lib.Identity.Data.Entries;
using Microsoft.EntityFrameworkCore;

namespace Haihv.DatDai.Lib.Identity.Data.Services;

public class IdentityDbContext : BaseDbContext
{
    public DbSet<User> Users { get; init; }
    public DbSet<Group> Groups { get; init; }
    public DbSet<Role> Roles { get; init; }
    public DbSet<UserGroup> UserGroups { get; init; }
    
    public IdentityDbContext(string connectionString, ElasticsearchClientSettings elasticsearchClientSettings) : 
        base(connectionString, elasticsearchClientSettings)
    { }
    public IdentityDbContext(string connectionString, AuditDataProvider auditDataProvider) : 
        base(connectionString, auditDataProvider)
    { }
}