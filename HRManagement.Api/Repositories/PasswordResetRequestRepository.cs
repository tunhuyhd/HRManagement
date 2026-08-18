using HRManagement.Api.Constants;
using HRManagement.Api.Data;
using HRManagement.Api.Entities;
using HRManagement.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HRManagement.Api.Repositories;

public sealed class PasswordResetRequestRepository(ApplicationDbContext dbContext)
    : IPasswordResetRequestRepository
{
    public async Task AddAsync(PasswordResetRequest request) =>
        await dbContext.PasswordResetRequests.AddAsync(request);

    public Task<bool> HasPendingRequestAsync(Guid userId) =>
        dbContext.PasswordResetRequests.AnyAsync(request =>
            request.UserId == userId && request.Status == PasswordResetStatus.Pending);

    public Task<PasswordResetRequest?> GetByIdAsync(Guid id) =>
        dbContext.PasswordResetRequests
            .AsNoTracking()
            .Include(request => request.User)
            .FirstOrDefaultAsync(request => request.Id == id);

    public Task<PasswordResetRequest?> GetByIdForUpdateAsync(Guid id) =>
        dbContext.PasswordResetRequests
            .Include(request => request.User)
            .FirstOrDefaultAsync(request => request.Id == id);

    public async Task<(IReadOnlyList<PasswordResetRequest> Items, int TotalCount)>
        GetListAsync(
            int pageNumber,
            int pageSize,
            string? search,
            PasswordResetStatus? status)
    {
        var query = dbContext.PasswordResetRequests
            .AsNoTracking()
            .Include(request => request.User)
            .AsQueryable();

        if (status.HasValue)
        {
            query = query.Where(request => request.Status == status.Value);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var keyword = search.Trim();
            query = query.Where(request =>
                request.User.Email != null &&
                EF.Functions.ILike(request.User.Email, $"%{keyword}%"));
        }

        var totalCount = await query.CountAsync();
        var items = await query
            .OrderBy(request => request.Status)
            .ThenByDescending(request => request.CreatedAtUtc)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public Task SaveChangesAsync() => dbContext.SaveChangesAsync();
}
