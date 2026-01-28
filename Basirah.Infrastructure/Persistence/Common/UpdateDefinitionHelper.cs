using System.Reflection;
using MongoDB.Driver;

namespace Basirah.Infrastructure.Persistence.Common;

/// <summary>
/// Helper for building MongoDB update definitions dynamically.
/// </summary>
public static class UpdateDefinitionHelper
{
    /// <summary>
    /// Create an update definition from an entity, excluding specified properties.
    /// </summary>
    public static UpdateDefinition<T> FromEntity<T>(T entity, params string[] excludeProperties) where T : class
    {
        var excludeSet = new HashSet<string>(excludeProperties, StringComparer.OrdinalIgnoreCase);
        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanRead && !excludeSet.Contains(p.Name));

        var updates = new List<UpdateDefinition<T>>();

        foreach (var property in properties)
        {
            var value = property.GetValue(entity);
            var update = Builders<T>.Update.Set(property.Name, value);
            updates.Add(update);
        }

        return Builders<T>.Update.Combine(updates);
    }

    /// <summary>
    /// Create an update definition for specific properties only.
    /// </summary>
    public static UpdateDefinition<T> ForProperties<T>(T entity, params string[] includeProperties) where T : class
    {
        var includeSet = new HashSet<string>(includeProperties, StringComparer.OrdinalIgnoreCase);
        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanRead && includeSet.Contains(p.Name));

        var updates = new List<UpdateDefinition<T>>();

        foreach (var property in properties)
        {
            var value = property.GetValue(entity);
            var update = Builders<T>.Update.Set(property.Name, value);
            updates.Add(update);
        }

        return Builders<T>.Update.Combine(updates);
    }
}
