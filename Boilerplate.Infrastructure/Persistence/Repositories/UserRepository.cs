using Boilerplate.Application.Interfaces.Repositories;
using Boilerplate.Domain.Entities;
using Boilerplate.Infrastructure.Persistence.Common;
using MongoDB.Driver;

namespace Boilerplate.Infrastructure.Persistence.Repositories;

/// <summary>
/// User repository implementation for MongoDB.
/// </summary>
public class UserRepository(IMongoDbContext context) : IUserRepository
{
    private readonly IMongoCollection<User> _collection = context.GetCollection<User>();

    public async Task<User?> GetByIdAsync(Guid id)
    {
        var filter = FilterDefinitionHelper.ByIdNotDeleted<User>(id);
        return await _collection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        var filter = Builders<User>.Filter.Eq(x => x.Email, email)
            .AndNotDeleted();
        return await _collection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<List<User>> GetAllAsync()
    {
        var filter = FilterDefinitionHelper.NotDeleted<User>();
        return await _collection.Find(filter).ToListAsync();
    }

    public async Task<User> CreateAsync(User user)
    {
        await _collection.InsertOneAsync(user);
        return user;
    }

    public async Task<User> UpdateAsync(User user)
    {
        var filter = FilterDefinitionHelper.ById<User>(user.Id);
        var update = UpdateDefinitionHelper.FromEntity(user, nameof(User.Id), nameof(User.CreatedAt));
        
        await _collection.UpdateOneAsync(filter, update);
        return user;
    }

    public async Task DeleteAsync(Guid id)
    {
        var filter = FilterDefinitionHelper.ById<User>(id);
        await _collection.DeleteOneAsync(filter);
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        var filter = FilterDefinitionHelper.ByIdNotDeleted<User>(id);
        return await _collection.CountDocumentsAsync(filter) > 0;
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        var filter = Builders<User>.Filter.Eq(x => x.Email, email)
            .AndNotDeleted();
        return await _collection.CountDocumentsAsync(filter) > 0;
    }
}
