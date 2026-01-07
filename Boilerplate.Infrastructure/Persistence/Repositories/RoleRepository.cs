using Boilerplate.Application.Interfaces.Repositories;
using Boilerplate.Domain.Entities;
using Boilerplate.Infrastructure.Persistence.Common;
using MongoDB.Driver;

namespace Boilerplate.Infrastructure.Persistence.Repositories;

/// <summary>
/// Role repository implementation for MongoDB.
/// </summary>
public class RoleRepository(IMongoDbContext context) : IRoleRepository
{
    private readonly IMongoCollection<Role> _collection = context.GetCollection<Role>();

    public async Task<Role?> GetByIdAsync(Guid id)
    {
        var filter = FilterDefinitionHelper.ByIdNotDeleted<Role>(id);
        return await _collection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<Role?> GetByNameAsync(string name)
    {
        var filter = Builders<Role>.Filter.Eq(x => x.Name, name)
            .AndNotDeleted();
        return await _collection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<List<Role>> GetAllAsync()
    {
        var filter = FilterDefinitionHelper.NotDeleted<Role>();
        return await _collection.Find(filter).ToListAsync();
    }

    public async Task<Role> CreateAsync(Role role)
    {
        await _collection.InsertOneAsync(role);
        return role;
    }

    public async Task<Role> UpdateAsync(Role role)
    {
        var filter = FilterDefinitionHelper.ById<Role>(role.Id);
        var update = UpdateDefinitionHelper.FromEntity(role, nameof(Role.Id), nameof(Role.CreatedAt));
        
        await _collection.UpdateOneAsync(filter, update);
        return role;
    }

    public async Task DeleteAsync(Guid id)
    {
        var filter = FilterDefinitionHelper.ById<Role>(id);
        await _collection.DeleteOneAsync(filter);
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        var filter = FilterDefinitionHelper.ByIdNotDeleted<Role>(id);
        return await _collection.CountDocumentsAsync(filter) > 0;
    }

    public async Task<bool> NameExistsAsync(string name)
    {
        var filter = Builders<Role>.Filter.Eq(x => x.Name, name)
            .AndNotDeleted();
        return await _collection.CountDocumentsAsync(filter) > 0;
    }
}
