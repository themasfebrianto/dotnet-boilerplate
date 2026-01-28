using Basirah.Application.Interfaces.Repositories;
using Basirah.Domain.Entities;
using Basirah.Infrastructure.Persistence.Common;
using Basirah.Infrastructure.Persistence.Repositories.Common;
using MongoDB.Driver;

namespace Basirah.Infrastructure.Persistence.Repositories;

/// <summary>
/// Role repository implementation for MongoDB.
/// Extends MongoRepositoryBase for standard CRUD, only implements entity-specific methods.
/// </summary>
public class RoleRepository(IMongoDbContext context) 
    : MongoRepositoryBase<Role>(context), IRoleRepository
{
    /// <summary>
    /// Get role by name.
    /// </summary>
    public async Task<Role?> GetByNameAsync(string name)
    {
        var filter = Builders<Role>.Filter.Eq(x => x.Name, name);
        return await FindOneAsync(filter);
    }

    /// <summary>
    /// Check if a role name is already registered.
    /// </summary>
    public async Task<bool> NameExistsAsync(string name)
    {
        var filter = Builders<Role>.Filter.Eq(x => x.Name, name);
        return await AnyAsync(filter);
    }
}
