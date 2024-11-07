using Haihv.DatDai.Lib.Service.Logger.MongoDb.Entries;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Haihv.DatDai.Lib.Service.Logger.MongoDb.Repositories;

public class LogUserRepository(IMongoDbContext mongoDbContext) : IBaseRepository<LogUserEntry>
{
    private readonly IMongoCollection<LogUserEntry> _collection = mongoDbContext.LogUserEntries;
    public async Task<LogUserEntry?> GetByIdAsync(ObjectId id)
    {
        return await _collection.Find(x => x.Id == id).FirstOrDefaultAsync();
    }

    public IQueryable<LogUserEntry> GetQueryable()
    {
        return _collection.AsQueryable();
    }

    public async Task<LogUserEntry> UpdateAsync(LogUserEntry entry)
    {
        var findOptions = new FindOneAndUpdateOptions<LogUserEntry>
        {
            ReturnDocument = ReturnDocument.After
        };
        var filter = Builders<LogUserEntry>.Filter.Eq(x => x.Id, entry.Id);
        var update = Builders<LogUserEntry>.Update
            .Set(x => x.UserName, entry.UserName)
            .Set(x => x.UserId, entry.UserId)
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

    public async Task<IEnumerable<LogUserEntry>> CreateOrUpdateAsync(IEnumerable<LogUserEntry> entries, int bulkSize = 100, CancellationToken cancellationToken = default)
    {
        var result = new List<LogUserEntry>();
        var bulk = new List<WriteModel<LogUserEntry>>();
        foreach (var entry in entries)
        {
            var existingEntry = await GetByIdAsync(entry.Id);
            if (existingEntry == null)
            {
                bulk.Add(new InsertOneModel<LogUserEntry>(entry));
            }
            else
            {
                bulk.Add(new ReplaceOneModel<LogUserEntry>(Builders<LogUserEntry>.Filter.Eq(x => x.Id, entry.Id), entry));
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