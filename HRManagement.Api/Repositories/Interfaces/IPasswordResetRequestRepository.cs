using HRManagement.Api.Constants;
using HRManagement.Api.Entities;

namespace HRManagement.Api.Repositories.Interfaces;

public interface IPasswordResetRequestRepository
{
    Task AddAsync(PasswordResetRequest request);
    Task<bool> HasPendingRequestAsync(Guid userId);
    Task<PasswordResetRequest?> GetByIdAsync(Guid id);
    Task<PasswordResetRequest?> GetByIdForUpdateAsync(Guid id);
    Task<(IReadOnlyList<PasswordResetRequest> Items, int TotalCount)> GetListAsync(
        int pageNumber,
        int pageSize,
        string? search,
        PasswordResetStatus? status);
    Task SaveChangesAsync();
}
