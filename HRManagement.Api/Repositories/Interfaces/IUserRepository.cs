using Microsoft.AspNetCore.Identity;
using HRManagement.Api.Entities;

namespace HRManagement.Api.Repositories.Interfaces;

public interface IUserRepository
{
    Task<AppUser?> FindByEmailAsync(string email);
    Task<AppUser?> FindByIdAsync(Guid id);
    Task<IdentityResult> CreateAsync(AppUser user, string password);
    Task<IdentityResult> AddToRoleAsync(AppUser user, string role);
    Task<IdentityResult> RemoveFromRolesAsync(AppUser user, IEnumerable<string> roles);
    Task<IdentityResult> UpdateAsync(AppUser user);
    Task<IList<string>> GetRolesAsync(AppUser user);
    Task<bool> CheckPasswordAsync(AppUser user, string password);
}
