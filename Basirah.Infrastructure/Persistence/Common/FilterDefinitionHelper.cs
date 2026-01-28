using System.Linq.Expressions;
using Basirah.Domain.Common;
using MongoDB.Driver;

namespace Basirah.Infrastructure.Persistence.Common;

/// <summary>
/// Helper for building MongoDB filter definitions.
/// </summary>
public static class FilterDefinitionHelper
{
    /// <summary>
    /// Create a filter by ID.
    /// </summary>
    public static FilterDefinition<T> ById<T>(Guid id) where T : BaseEntity
    {
        return Builders<T>.Filter.Eq(x => x.Id, id);
    }
}
