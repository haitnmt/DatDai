using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Haihv.DatDai.Lib.Data.Base.Entities;

namespace Haihv.DatDai.Lib.Data.Base.Extensions;

public static class SoftDeletedExtensions
{
    /// <summary>
    /// Đánh dấu entity là soft deleted.
    /// </summary>
    public static void SoftDelete<T>(this DbSet<T> dbSet, T entity) where T : SoftDeletable
    {
        entity.IsDeleted = true;
        entity.DeletedAt = DateTimeOffset.UtcNow;
        dbSet.Update(entity);
    }
    /// <summary>
    /// Đánh dấu entity là soft deleted.
    /// </summary>
    public static bool SoftDelete<T>(this DbSet<T> dbSet, ICollection<T> entities) where T : SoftDeletable
    {
        if (entities.Count == 0)
        {
            return false;
        }
        foreach (var entity in entities)
        {
            entity.IsDeleted = true;
            entity.DeletedAt = DateTimeOffset.UtcNow;
            dbSet.Update(entity);
        }
        return true;
    }
    
    /// <summary>
    /// Đánh dấu entity là hard deleted.
    /// </summary>
    public static void HardDelete<T>(this DbSet<T> dbSet, T entity) where T : SoftDeletable
    {
        if (!entity.IsDeleted)
        {
            throw new InvalidOperationException("Cannot hard delete an entity that is not soft deleted.");
        }
        dbSet.Remove(entity);
    }
    

    /// <summary>
    /// Áp dụng global filter cho tất cả các entity kế thừa từ SoftDeletable.
    /// </summary>
    public static void ApplySoftDeleteQueryFilter(this ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(SoftDeletable).IsAssignableFrom(entityType.ClrType))
            {
                var parameter = Expression.Parameter(entityType.ClrType, "entity");
                var property = Expression.Call(
                    typeof(EF), 
                    nameof(EF.Property), 
                    [typeof(bool)], 
                    parameter, 
                    Expression.Constant("IsDeleted")
                );
                var filter = Expression.Lambda(Expression.Not(property), parameter);
                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(filter);
            }
        }
    }
}