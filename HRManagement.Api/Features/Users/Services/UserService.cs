using HRManagement.Api.Constants;
using HRManagement.Api.Entities;
using HRManagement.Api.Features.Users.Models;
using HRManagement.Api.Repositories.Interfaces;

namespace HRManagement.Api.Features.Users.Services;

public sealed class UserService(IUserRepository userRepository) : IUserService
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
}
