using Boilerplate.Application.Interfaces.Repositories;
using Boilerplate.Domain.Entities;
using Boilerplate.Infrastructure.Persistence.Common;
using Boilerplate.Infrastructure.Persistence.Repositories.Common;
using MongoDB.Driver;

namespace Boilerplate.Infrastructure.Persistence.Repositories;

/// <summary>
/// User repository implementation for MongoDB.
/// Extends MongoRepositoryBase for standard CRUD, only implements entity-specific methods.
/// </summary>
public class UserRepository(IMongoDbContext context) 
    : MongoRepositoryBase<User>(context), IUserRepository
{
    /// <summary>
    /// Get user by email address, excluding soft-deleted records.
    /// </summary>
    public async Task<User?> GetByEmailAsync(string email)
    {
        var filter = Builders<User>.Filter.Eq(x => x.Email, email);
        return await FindOneAsync(filter);
    }

    /// <summary>
    /// Check if an email address is already registered.
    /// </summary>
    public async Task<bool> EmailExistsAsync(string email)
    {
        var filter = Builders<User>.Filter.Eq(x => x.Email, email);
        return await AnyAsync(filter);
    }
}
