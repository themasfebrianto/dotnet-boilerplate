using Basirah.Application.Interfaces.Repositories;
using Basirah.Domain.Entities;
using MongoDB.Driver;

namespace Basirah.Infrastructure.Persistence.Repositories;

/// <summary>
/// Refresh token repository implementation for MongoDB.
/// </summary>
public class RefreshTokenRepository(IMongoDbContext context) : IRefreshTokenRepository
{
    private readonly IMongoCollection<RefreshToken> _collection = context.GetCollection<RefreshToken>();

    public async Task<RefreshToken?> GetByTokenAsync(string token)
    {
        var filter = Builders<RefreshToken>.Filter.Eq(x => x.Token, token);
        return await _collection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<RefreshToken?> GetByUserIdAsync(Guid userId)
    {
        var filter = Builders<RefreshToken>.Filter.Eq(x => x.UserId, userId);
        return await _collection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task CreateAsync(RefreshToken refreshToken)
    {
        await _collection.InsertOneAsync(refreshToken);
    }

    public async Task UpdateAsync(RefreshToken refreshToken)
    {
        var filter = Builders<RefreshToken>.Filter.Eq(x => x.Id, refreshToken.Id);
        await _collection.ReplaceOneAsync(filter, refreshToken);
    }

    public async Task DeleteByUserIdAsync(Guid userId)
    {
        var filter = Builders<RefreshToken>.Filter.Eq(x => x.UserId, userId);
        await _collection.DeleteManyAsync(filter);
    }

    public async Task RevokeAsync(string token, string? revokedByIp = null)
    {
        var filter = Builders<RefreshToken>.Filter.Eq(x => x.Token, token);
        var update = Builders<RefreshToken>.Update
            .Set(x => x.RevokedAt, DateTime.UtcNow)
            .Set(x => x.RevokedByIp, revokedByIp);
        
        await _collection.UpdateOneAsync(filter, update);
    }

    public async Task UpsertAsync(Guid userId, string token, DateTime expiresAt, string? createdByIp = null)
    {
        var filter = Builders<RefreshToken>.Filter.Eq(x => x.UserId, userId);
        var update = Builders<RefreshToken>.Update
            .Set(x => x.Token, token)
            .Set(x => x.ExpiresAt, expiresAt)
            .Set(x => x.CreatedByIp, createdByIp)
            .Set(x => x.RevokedAt, null)
            .Set(x => x.RevokedByIp, null)
            .Set(x => x.ReplacedByToken, null)
            .SetOnInsert(x => x.UserId, userId)
            .SetOnInsert(x => x.CreatedAt, DateTime.UtcNow);

        await _collection.UpdateOneAsync(filter, update, new UpdateOptions { IsUpsert = true });
    }
}
