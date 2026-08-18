using Microsoft.AspNetCore.Identity;
using HRManagement.Api.Entities;
using HRManagement.Api.Repositories.Interfaces;

namespace HRManagement.Api.Repositories;

public sealed class UserRepository(UserManager<AppUser> userManager) : IUserRepository
{
    public Task<AppUser?> FindByEmailAsync(string email) => userManager.FindByEmailAsync(email);

    public Task<IdentityResult> CreateAsync(AppUser user, string password) =>
        userManager.CreateAsync(user, password);

    public Task<IdentityResult> AddToRoleAsync(AppUser user, string role) =>
        userManager.AddToRoleAsync(user, role);

    public Task<IList<string>> GetRolesAsync(AppUser user) => userManager.GetRolesAsync(user);

    public Task<bool> CheckPasswordAsync(AppUser user, string password) =>
        userManager.CheckPasswordAsync(user, password);
}
