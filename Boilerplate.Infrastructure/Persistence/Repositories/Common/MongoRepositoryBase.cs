using Boilerplate.Domain.Common;
using Boilerplate.Domain.Common.Interfaces;
using Boilerplate.Infrastructure.Persistence.Common;
using MongoDB.Driver;

namespace Boilerplate.Infrastructure.Persistence.Repositories.Common;

/// <summary>
/// Generic base repository for MongoDB with built-in soft-delete support.
/// Reduces boilerplate for standard CRUD operations.
/// </summary>
/// <typeparam name="T">Entity type that must extend BaseEntity and implement ISoftDeletable.</typeparam>
public abstract class MongoRepositoryBase<T> where T : BaseEntity, ISoftDeletable
{
    protected readonly IMongoCollection<T> Collection;

    protected MongoRepositoryBase(IMongoDbContext context, string? collectionName = null)
    {
        Collection = context.GetCollection<T>(collectionName);
    }

    /// <summary>
    /// Get entity by ID, excluding soft-deleted records.
    /// </summary>
    public virtual async Task<T?> GetByIdAsync(Guid id)
    {
        var filter = FilterDefinitionHelper.ByIdNotDeleted<T>(id);
        return await Collection.Find(filter).FirstOrDefaultAsync();
    }

    /// <summary>
    /// Get all entities, excluding soft-deleted records.
    /// </summary>
    public virtual async Task<List<T>> GetAllAsync()
    {
        var filter = FilterDefinitionHelper.NotDeleted<T>();
        return await Collection.Find(filter).ToListAsync();
    }

    /// <summary>
    /// Create a new entity.
    /// </summary>
    public virtual async Task<T> CreateAsync(T entity)
    {
        entity.Id = Guid.NewGuid();
        entity.CreatedAt = DateTime.UtcNow;
        await Collection.InsertOneAsync(entity);
        return entity;
    }

    /// <summary>
    /// Update an existing entity.
    /// </summary>
    public virtual async Task<T> UpdateAsync(T entity)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        var filter = FilterDefinitionHelper.ById<T>(entity.Id);
        var update = UpdateDefinitionHelper.FromEntity(entity, nameof(BaseEntity.Id), nameof(BaseEntity.CreatedAt));
        
        await Collection.UpdateOneAsync(filter, update);
        return entity;
    }

    /// <summary>
    /// Hard delete an entity by ID.
    /// </summary>
    public virtual async Task DeleteAsync(Guid id)
    {
        var filter = FilterDefinitionHelper.ById<T>(id);
        await Collection.DeleteOneAsync(filter);
    }

    /// <summary>
    /// Check if an entity exists by ID, excluding soft-deleted records.
    /// </summary>
    public virtual async Task<bool> ExistsAsync(Guid id)
    {
        var filter = FilterDefinitionHelper.ByIdNotDeleted<T>(id);
        return await Collection.CountDocumentsAsync(filter) > 0;
    }

    /// <summary>
    /// Find entities matching a filter, excluding soft-deleted records.
    /// </summary>
    protected async Task<List<T>> FindAsync(FilterDefinition<T> filter)
    {
        return await Collection.Find(filter.AndNotDeleted()).ToListAsync();
    }

    /// <summary>
    /// Find a single entity matching a filter, excluding soft-deleted records.
    /// </summary>
    protected async Task<T?> FindOneAsync(FilterDefinition<T> filter)
    {
        return await Collection.Find(filter.AndNotDeleted()).FirstOrDefaultAsync();
    }

    /// <summary>
    /// Check if any entity matches a filter, excluding soft-deleted records.
    /// </summary>
    protected async Task<bool> AnyAsync(FilterDefinition<T> filter)
    {
        return await Collection.CountDocumentsAsync(filter.AndNotDeleted()) > 0;
    }
}
