using Haihv.DatDai.Lib.Service.Logger.MongoDb.Entries;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Haihv.DatDai.Lib.Service.Logger.MongoDb.Repositories;

public class AuditRepository(IMongoDbContext mongoDbContext) : IBaseRepository<AuditEntry>
{
    private readonly IMongoCollection<AuditEntry> _collection = mongoDbContext.AuditEntries;
    public async Task<AuditEntry?> GetByIdAsync(ObjectId id)
    {
        return await _collection.Find(x => x.Id == id).FirstOrDefaultAsync();
    }

    public IQueryable<AuditEntry> GetQueryable()
    {
        return _collection.AsQueryable();
    }

    public async Task<AuditEntry> UpdateAsync(AuditEntry entry)
    {
        var findOptions = new FindOneAndUpdateOptions<AuditEntry>
        {
            ReturnDocument = ReturnDocument.After
        };
        var filter = Builders<AuditEntry>.Filter.Or(Builders<AuditEntry>.Filter.Eq(x => x.Id, entry.Id),
            Builders<AuditEntry>.Filter.And(
                Builders<AuditEntry>.Filter.Ne(x => x.Hash, string.Empty),
                Builders<AuditEntry>.Filter.Eq(x => x.Hash, entry.Hash)));
        var update = Builders<AuditEntry>.Update
            .Set(x => x.Metadata, entry.Metadata)
            .Set(x => x.StartTimeUtc, entry.StartTimeUtc)
            .Set(x => x.EndTimeUtc, entry.EndTimeUtc)
            .Set(x => x.Success, entry.Success)
            .Set(x => x.Exception, entry.Exception);
        return await _collection.FindOneAndUpdateAsync(filter, update, findOptions);
    }
    public async Task DeleteAsync(ObjectId id)
    {
        await _collection.DeleteOneAsync(x => x.Id == id);
    }
    
    public async Task<IEnumerable<AuditEntry>> CreateOrUpdateAsync(IEnumerable<AuditEntry> entries, int bulkSize = 100, CancellationToken cancellationToken = default)
    {
        var result = new List<AuditEntry>();
        var bulk = new List<WriteModel<AuditEntry>>();
        foreach (var entry in entries)
        {
            var existingEntry = await GetByIdAsync(entry.Id) ??
                                await _collection.Find(x => x.Hash == entry.Hash)
                                    .FirstOrDefaultAsync(cancellationToken: cancellationToken);
            if (existingEntry == null)
            {
                bulk.Add(new InsertOneModel<AuditEntry>(entry));
            }
            else
            {
                entry.StartTimeUtc = existingEntry.StartTimeUtc;
                bulk.Add(new ReplaceOneModel<AuditEntry>(Builders<AuditEntry>.Filter.Eq(x => x.Id, entry.Id), entry));
            }
            result.Add(entry);
            if (bulk.Count < bulkSize) continue;
            await _collection.BulkWriteAsync(bulk, cancellationToken: cancellationToken);
            bulk.Clear();
        }
        if (bulk.Count > 0)
        {
            await _collection.BulkWriteAsync(bulk, cancellationToken: cancellationToken);
        }
        return result;
    }
}