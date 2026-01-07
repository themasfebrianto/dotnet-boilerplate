using System.Linq.Expressions;
using Boilerplate.Domain.Common;
using Boilerplate.Domain.Common.Interfaces;
using MongoDB.Driver;

namespace Boilerplate.Infrastructure.Persistence.Common;

/// <summary>
/// Helper for building MongoDB filter definitions.
/// </summary>
public static class FilterDefinitionHelper
{
    /// <summary>
    /// Create a filter that excludes soft-deleted entities.
    /// </summary>
    public static FilterDefinition<T> NotDeleted<T>() where T : ISoftDeletable
    {
        return Builders<T>.Filter.Eq(x => x.DeletedAt, null);
    }

    /// <summary>
    /// Combine a filter with the not-deleted filter.
    /// </summary>
    public static FilterDefinition<T> AndNotDeleted<T>(this FilterDefinition<T> filter) where T : ISoftDeletable
    {
        return Builders<T>.Filter.And(filter, NotDeleted<T>());
    }

    /// <summary>
    /// Create a filter by ID.
    /// </summary>
    public static FilterDefinition<T> ById<T>(Guid id) where T : BaseEntity
    {
        return Builders<T>.Filter.Eq(x => x.Id, id);
    }

    /// <summary>
    /// Create a filter by ID that excludes soft-deleted entities.
    /// </summary>
    public static FilterDefinition<T> ByIdNotDeleted<T>(Guid id) where T : BaseEntity, ISoftDeletable
    {
        return Builders<T>.Filter.And(ById<T>(id), NotDeleted<T>());
    }
}
