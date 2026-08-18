using HRManagement.Api.Data;
using HRManagement.Api.Entities;
using HRManagement.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HRManagement.Api.Repositories;

public sealed class RefreshTokenRepository(ApplicationDbContext dbContext)
    : IRefreshTokenRepository
{
    public async Task AddAsync(RefreshToken refreshToken) =>
        await dbContext.RefreshTokens.AddAsync(refreshToken);

    public Task<RefreshToken?> GetByHashForUpdateAsync(string tokenHash) =>
        dbContext.RefreshTokens.FirstOrDefaultAsync(token => token.TokenHash == tokenHash);

    public async Task<IReadOnlyList<RefreshToken>> GetActiveByUserIdAsync(
        Guid userId,
        DateTime nowUtc) =>
        await dbContext.RefreshTokens
            .Where(token =>
                token.UserId == userId &&
                token.RevokedAtUtc == null &&
                token.ExpiresAtUtc > nowUtc)
            .ToListAsync();

    public Task SaveChangesAsync() => dbContext.SaveChangesAsync();
}
