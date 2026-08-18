using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using HRManagement.Api.Constants;
using HRManagement.Api.Entities;

namespace HRManagement.Api.Data.Seeding;

public static class DatabaseInitializer
{
    public static async Task InitializeDatabaseAsync(this IServiceProvider services)
    {
        await using var scope = services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();

        await dbContext.Database.MigrateAsync();

        foreach (var roleName in AppRoles.All)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                var roleResult = await roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
                EnsureSucceeded(roleResult, $"create role {roleName}");
            }
        }

        const string adminEmail = "admin@gmail.com";
        var admin = await userManager.FindByEmailAsync(adminEmail);
        if (admin is null)
        {
            admin = new AppUser
            {
                Id = Guid.NewGuid(),
                Email = adminEmail,
                UserName = adminEmail,
                EmailConfirmed = true
            };

            var createResult = await userManager.CreateAsync(admin, "admin123");
            EnsureSucceeded(createResult, "create seeded admin user");
        }

        if (!await userManager.IsInRoleAsync(admin, AppRoles.Admin))
        {
            var roleResult = await userManager.AddToRoleAsync(admin, AppRoles.Admin);
            EnsureSucceeded(roleResult, "assign ADMIN role to seeded user");
        }
    }

    private static void EnsureSucceeded(IdentityResult result, string operation)
    {
        if (result.Succeeded)
        {
            return;
        }

        var errors = string.Join("; ", result.Errors.Select(error => error.Description));
        throw new InvalidOperationException($"Failed to {operation}: {errors}");
    }
}
