using HRManagement.Api.Constants;
using HRManagement.Api.Entities;
using HRManagement.Api.Features.Users.Models;
using HRManagement.Api.Repositories.Interfaces;

namespace HRManagement.Api.Features.Users.Services;

public sealed class UserService(
    IUserRepository userRepository,
    IRefreshTokenRepository refreshTokenRepository) : IUserService
{
    public async Task<CreateUserResult> CreateAsync(CreateUserRequest request)
    {
        var normalizedEmail = request.Email.Trim();
        if (await userRepository.FindByEmailAsync(normalizedEmail) is not null)
        {
            return new CreateUserResult(null, true, ["Email is already registered."]);
        }

        var user = new AppUser
        {
            Id = Guid.NewGuid(),
            Email = normalizedEmail,
            UserName = normalizedEmail,
            EmailConfirmed = true
        };

        var createResult = await userRepository.CreateAsync(user, request.Password);
        if (!createResult.Succeeded)
        {
            return new CreateUserResult(
                null,
                false,
                createResult.Errors.Select(error => error.Description).ToArray());
        }

        var roleResult = await userRepository.AddToRoleAsync(user, AppRoles.User);
        if (!roleResult.Succeeded)
        {
            return new CreateUserResult(
                null,
                false,
                roleResult.Errors.Select(error => error.Description).ToArray());
        }

        return new CreateUserResult(
            new CreateUserResponse(user.Id, user.Email!, AppRoles.User),
            false,
            Array.Empty<string>());
    }

    public async Task<UpdateUserAccessResult> UpdateAccessAsync(
        Guid id,
        UpdateUserAccessRequest request)
    {
        var user = await userRepository.FindByIdAsync(id);
        if (user is null)
        {
            return new UpdateUserAccessResult(null, UpdateUserAccessError.UserNotFound);
        }

        var currentRoles = await userRepository.GetRolesAsync(user);
        if (currentRoles.Contains(AppRoles.Admin, StringComparer.OrdinalIgnoreCase))
        {
            return new UpdateUserAccessResult(null, UpdateUserAccessError.AdminUserProtected);
        }

        var targetRole = request.Role.Trim().ToUpperInvariant();
        var rolesToRemove = currentRoles
            .Where(role => AppRoles.AssignableByAdmin.Contains(role, StringComparer.OrdinalIgnoreCase))
            .Where(role => !role.Equals(targetRole, StringComparison.OrdinalIgnoreCase))
            .ToArray();

        if (rolesToRemove.Length > 0)
        {
            var removeResult = await userRepository.RemoveFromRolesAsync(user, rolesToRemove);
            if (!removeResult.Succeeded)
            {
                return IdentityFailure(removeResult);
            }
        }

        if (!currentRoles.Contains(targetRole, StringComparer.OrdinalIgnoreCase))
        {
            var addResult = await userRepository.AddToRoleAsync(user, targetRole);
            if (!addResult.Succeeded)
            {
                foreach (var removedRole in rolesToRemove)
                {
                    await userRepository.AddToRoleAsync(user, removedRole);
                }

                return IdentityFailure(addResult);
            }
        }

        user.IsActive = request.IsActive;
        var updateResult = await userRepository.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            return IdentityFailure(updateResult);
        }

        if (!user.IsActive)
        {
            var nowUtc = DateTime.UtcNow;
            var activeTokens = await refreshTokenRepository.GetActiveByUserIdAsync(user.Id, nowUtc);

            foreach (var token in activeTokens)
            {
                token.RevokedAtUtc = nowUtc;
            }

            await refreshTokenRepository.SaveChangesAsync();
        }

        return new UpdateUserAccessResult(
            new UserAccessResponse(user.Id, user.Email!, targetRole, user.IsActive));
    }

    private static UpdateUserAccessResult IdentityFailure(
        Microsoft.AspNetCore.Identity.IdentityResult result) =>
        new(
            null,
            UpdateUserAccessError.IdentityError,
            result.Errors.Select(error => error.Description).ToArray());
}
