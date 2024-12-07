namespace Haihv.DatDai.Lib.Data.Base.Entities;

public interface IBaseService<T> where T : class
{
    Task<bool> DeleteByIdAsync(Guid id);
    Task<T?> GetByIdAsync(Guid id);
    Task<IEnumerable<T>> GetAllAsync();
}