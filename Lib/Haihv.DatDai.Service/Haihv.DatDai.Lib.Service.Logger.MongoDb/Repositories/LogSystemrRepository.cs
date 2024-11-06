using Haihv.DatDai.Lib.Service.Logger.MongoDb.Entries;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Haihv.DatDai.Lib.Service.Logger.MongoDb.Repositories;

public class LogSystemrRepository(IMongoDbContext mongoDbContext) : IBaseRepository<LogSystemEntry>
{
    private readonly IMongoCollection<LogSystemEntry> _collection = mongoDbContext.LogSystemEntries;
    public async Task<LogSystemEntry?> GetByIdAsync(ObjectId id)
    {
        return await _collection.Find(x => x.Id == id).FirstOrDefaultAsync();
    }

    public IQueryable<LogSystemEntry> GetQueryable()
    {
        return _collection.AsQueryable();
    }

    public async Task<LogSystemEntry> UpdateAsync(LogSystemEntry entry)
    {
        var findOptions = new FindOneAndUpdateOptions<LogSystemEntry>
        {
            ReturnDocument = ReturnDocument.After
        };
        var filter = Builders<LogSystemEntry>.Filter.Eq(x => x.Id, entry.Id);
        var update = Builders<LogSystemEntry>.Update
            .Set(x => x.SystemName, entry.SystemName)
            .Set(x => x.SystemDescription, entry.SystemDescription)
            .Set(x => x.FunctionName, entry.FunctionName)
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

    public async Task<IEnumerable<LogSystemEntry>> CreateOrUpdateAsync(IEnumerable<LogSystemEntry> entries, int bulkSize = 100)
    {
        var result = new List<LogSystemEntry>();
        var bulk = new List<WriteModel<LogSystemEntry>>();
        foreach (var entry in entries)
        {
            var existingEntry = await GetByIdAsync(entry.Id);
            if (existingEntry == null)
            {
                bulk.Add(new InsertOneModel<LogSystemEntry>(entry));
            }
            else
            {
                bulk.Add(new ReplaceOneModel<LogSystemEntry>(Builders<LogSystemEntry>.Filter.Eq(x => x.Id, entry.Id), entry));
            }
            result.Add(entry);
            if (bulk.Count < bulkSize) continue;
            await _collection.BulkWriteAsync(bulk);
            bulk.Clear();
        }
        if (bulk.Count > 0)
        {
            await _collection.BulkWriteAsync(bulk);
        }
        return result;
    }
}
