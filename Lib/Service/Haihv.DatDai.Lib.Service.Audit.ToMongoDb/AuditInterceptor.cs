using Haihv.DatDai.Lib.Service.Logger.MongoDb;
using Haihv.DatDai.Lib.Service.Logger.MongoDb.Entries;
using Haihv.DatDai.Lib.Service.Logger.MongoDb.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Haihv.DatDai.Lib.Service.Audit.ToMongoDb;

public class AuditInterceptor(IMongoDbContext mongoDbContext) : SaveChangesInterceptor
{
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
        
        await _auditRepository.CreateOrUpdateAsync(auditEntries,cancellationToken: cancellationToken);
        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public override async ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData, int result, CancellationToken cancellationToken = default)
    {
        if (eventData.Context is null)
            return await base.SavedChangesAsync(eventData, result, cancellationToken);

        var endTime = DateTime.UtcNow;
        
        var auditEntries = eventData.Context.ChangeTracker
            .Entries()
            .Where(x => x.State is
                EntityState.Added or
                EntityState.Modified or
                EntityState.Deleted)
            .Select(x => new AuditEntry
            {
                EntryName = x.Entity.GetType().ToString(),
                Metadata = x.DebugView.LongView,
                EndTimeUtc = endTime,
                Success = true
            }).ToList();

        if (auditEntries.Count <= 0) return await base.SavedChangesAsync(eventData, result, cancellationToken);
        await _auditRepository.CreateOrUpdateAsync(auditEntries,cancellationToken: cancellationToken);
        return await base.SavedChangesAsync(eventData, result, cancellationToken);
    }

    public override async void SaveChangesFailed(DbContextErrorEventData eventData)
    {
        if (eventData.Context is null)
            return;

        var endTime = DateTime.UtcNow;
        var auditEntries = eventData.Context.ChangeTracker
            .Entries()
            .Where(x => x.State is
                EntityState.Added or
                EntityState.Modified or
                EntityState.Deleted)
            .Select(x => new AuditEntry
            {
                EntryName = x.Entity.GetType().ToString(),
                Metadata = x.DebugView.LongView,
                EndTimeUtc = endTime,
                Success = false,
                Exception = eventData.Exception.Message,
            }).ToList();

        // Save the audit entries to the database
        if (auditEntries.Count <= 0) return;

        await _auditRepository.CreateOrUpdateAsync(auditEntries);
    }
}