using Boilerplate.Application.Common.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Boilerplate.Infrastructure.Persistence;

/// <summary>
/// MongoDB database context interface.
/// </summary>
public interface IMongoDbContext
{
    IMongoCollection<T> GetCollection<T>(string? collectionName = null);
}

/// <summary>
/// MongoDB database context implementation.
/// </summary>
public class MongoDbContext : IMongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(IOptions<MongoDbSettings> settings)
    {
        var mongoClient = new MongoClient(settings.Value.ConnectionString);
        _database = mongoClient.GetDatabase(settings.Value.DatabaseName);
    }

    public IMongoCollection<T> GetCollection<T>(string? collectionName = null)
    {
        // Use provided name or derive from type name (pluralized)
        var name = collectionName ?? $"{typeof(T).Name}s";
        return _database.GetCollection<T>(name);
    }
}
