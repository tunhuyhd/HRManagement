using HRManagement.Api.Entities;

namespace HRManagement.Api.Repositories.Interfaces;

public interface IRefreshTokenRepository
{
    Task AddAsync(RefreshToken refreshToken);
    Task<RefreshToken?> GetByHashForUpdateAsync(string tokenHash);
    Task<IReadOnlyList<RefreshToken>> GetActiveByUserIdAsync(Guid userId, DateTime nowUtc);
    Task SaveChangesAsync();
}
