using Microsoft.AspNetCore.Identity;
using HRManagement.Api.Entities;
using HRManagement.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HRManagement.Api.Repositories;

public sealed class UserRepository(UserManager<AppUser> userManager) : IUserRepository
{
    public Task<AppUser?> FindByEmailAsync(string email) => userManager.FindByEmailAsync(email);

    public Task<AppUser?> FindByIdAsync(Guid id) => userManager.FindByIdAsync(id.ToString());

    public async Task<(IReadOnlyList<AppUser> Items, int TotalCount)> GetListAsync(
        int pageNumber,
        int pageSize,
        string? search)
    {
        var query = userManager.Users.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var keyword = search.Trim();
            query = query.Where(user =>
                user.Email != null && EF.Functions.ILike(user.Email, $"%{keyword}%"));
        }

        var totalCount = await query.CountAsync();
        var users = await query
            .OrderBy(user => user.Email)
            .ThenBy(user => user.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (users, totalCount);
    }

    public Task<IdentityResult> CreateAsync(AppUser user, string password) =>
        userManager.CreateAsync(user, password);

    public Task<IdentityResult> AddToRoleAsync(AppUser user, string role) =>
        userManager.AddToRoleAsync(user, role);

    public Task<IdentityResult> RemoveFromRolesAsync(AppUser user, IEnumerable<string> roles) =>
        userManager.RemoveFromRolesAsync(user, roles);

    public Task<IdentityResult> UpdateAsync(AppUser user) => userManager.UpdateAsync(user);

    public Task<IList<string>> GetRolesAsync(AppUser user) => userManager.GetRolesAsync(user);

    public Task<bool> CheckPasswordAsync(AppUser user, string password) =>
        userManager.CheckPasswordAsync(user, password);
}
