using MongoDB.Bson;

namespace Haihv.DatDai.Lib.Service.Logger.MongoDb.Repositories;

public interface IBaseRepository<T>
    where T : class
{
    Task<T?> GetByIdAsync(ObjectId id);
    
    IQueryable<T> GetQueryable();
    Task<T> UpdateAsync(T entry);
    Task DeleteAsync(ObjectId id);
    Task<IEnumerable<T>> CreateOrUpdateAsync(IEnumerable<T> entries, int bulkSize = 100, CancellationToken cancellationToken = default);
}