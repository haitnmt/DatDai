using Haihv.DatDai.Lib.Service.Logger.MongoDb;
using Haihv.DatDai.Lib.Service.Logger.MongoDb.Entries;
using Haihv.DatDai.Lib.Service.Logger.MongoDb.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Caching.Memory;

namespace Haihv.DatDai.Lib.Service.Audit.ToMongoDb;

public class AuditInterceptor(IMongoDbContext mongoDbContext, IMemoryCache memoryCache) : SaveChangesInterceptor
{
    private readonly IMemoryCache _memoryCache = memoryCache;
    private readonly AuditRepository _auditRepository = new(mongoDbContext);

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        if (eventData.Context is null)
        {
            return await base.SavingChangesAsync(eventData, result, cancellationToken);
        }
        var startTime = DateTime.UtcNow;
        
        var auditEntries = eventData.Context.ChangeTracker
            .Entries()
            .Where(x => x.State is
            EntityState.Added or
            EntityState.Modified or
            EntityState.Deleted)
            .Select(x => new AuditEntry
            {
                EntryName = x.Context.GetType().ToString(),
                StartTimeUtc = startTime,
                Metadata = x.DebugView.LongView,
            }).ToList();
        if (auditEntries.Count == 0)
        {
            return await base.SavingChangesAsync(eventData, result, cancellationToken);
        }
        _memoryCache.Set(eventData.Context.GetHashCode(), auditEntries);
        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public override async ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData, int result, CancellationToken cancellationToken = default)
    {
        if (eventData.Context is null)
            return await base.SavedChangesAsync(eventData, result, cancellationToken);
        
        var endTime = DateTime.UtcNow;
        var auditEntries = _memoryCache.Get<List<AuditEntry>>(eventData.Context.GetHashCode());
        if (auditEntries is null)
        {
            return await base.SavedChangesAsync(eventData, result, cancellationToken);
        }
        foreach (var auditEntry in auditEntries)
        {
            auditEntry.EndTimeUtc = endTime;
            auditEntry.Success = true;
        }
        if (auditEntries.Count <= 0) return await base.SavedChangesAsync(eventData, result, cancellationToken);
        await _auditRepository.CreateOrUpdateAsync(auditEntries,cancellationToken: cancellationToken);
        return await base.SavedChangesAsync(eventData, result, cancellationToken);
    }

    public override async void SaveChangesFailed(DbContextErrorEventData eventData)
    {
        if (eventData.Context is null)
            return;

        var endTime = DateTime.UtcNow;
        var auditEntries = _memoryCache.Get<List<AuditEntry>>(eventData.Context.GetHashCode());
        if (auditEntries is null)
        {
            return;
        }
        foreach (var auditEntry in auditEntries)
        {
            auditEntry.EndTimeUtc = endTime;
            auditEntry.Success = false;
            auditEntry.Exception = eventData.Exception.Message;
        }
        // Save the audit entries to the database
        if (auditEntries.Count <= 0) return;
        await _auditRepository.CreateOrUpdateAsync(auditEntries);
    }
}