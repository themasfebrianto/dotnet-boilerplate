using System.Reflection;
using Basirah.Domain.Common;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;

namespace Basirah.Infrastructure.Persistence;

/// <summary>
/// MongoDB BSON mappings for domain entities.
/// </summary>
public static class MongoMappings
{
    private static bool _registered;
    private static readonly object _lock = new();

    /// <summary>
    /// Register all entity mappings. Safe to call multiple times.
    /// </summary>
    public static void Register()
    {
        lock (_lock)
        {
            if (_registered) return;

            // Register global Guid serializer
            BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

            // Register BaseEntity first with Id mapping
            if (!BsonClassMap.IsClassMapRegistered(typeof(BaseEntity)))
            {
                BsonClassMap.RegisterClassMap<BaseEntity>(cm =>
                {
                    cm.AutoMap();
                    cm.MapIdMember(e => e.Id)
                        .SetSerializer(new GuidSerializer(GuidRepresentation.Standard))
                        .SetIdGenerator(GuidGenerator.Instance);
                });
            }

            // Auto-register all entities that inherit from BaseEntity
            var domainAssembly = typeof(BaseEntity).Assembly;

            var entityTypes = domainAssembly
                .GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && t != typeof(BaseEntity) && typeof(BaseEntity).IsAssignableFrom(t));

            foreach (var type in entityTypes)
            {
                if (BsonClassMap.IsClassMapRegistered(type)) continue;

                RegisterEntityType(type);
            }

            _registered = true;
        }
    }

    private static void RegisterEntityType(Type type)
    {
        // Create BsonClassMap<T> dynamically
        var classMapType = typeof(BsonClassMap<>).MakeGenericType(type);
        var classMap = (BsonClassMap)Activator.CreateInstance(classMapType)!;

        classMap.AutoMap();

        // Don't re-map Id - it's already mapped on BaseEntity
        // Just register the class map
        BsonClassMap.RegisterClassMap(classMap);
    }
}

