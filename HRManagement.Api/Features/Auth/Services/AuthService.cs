using HRManagement.Api.Common.Auth;
using HRManagement.Api.Features.Auth.Models;
using HRManagement.Api.Repositories.Interfaces;

namespace HRManagement.Api.Features.Auth.Services;

public sealed class AuthService(
    IUserRepository userRepository,
    IJwtTokenService jwtTokenService,
    ICurrentUser currentUser) : IAuthService
{
    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        var user = await userRepository.FindByEmailAsync(request.Email.Trim());
        if (user is null || !await userRepository.CheckPasswordAsync(user, request.Password))
        {
            return null;
        }

        var roles = await userRepository.GetRolesAsync(user);
        var (token, expiresAtUtc) = jwtTokenService.Create(user, roles);
        return new LoginResponse(token, expiresAtUtc, new UserResponse(user.Id, user.Email!, roles.ToArray()));
    }

    public async Task<UserResponse?> GetCurrentUserAsync()
    {
        if (currentUser.Id is not { } userId || currentUser.Email is null)
        {
            return null;
        }

        var user = await userRepository.FindByEmailAsync(currentUser.Email);
        if (user is null || user.Id != userId)
        {
            return null;
        }

        var roles = await userRepository.GetRolesAsync(user);
        return new UserResponse(user.Id, user.Email!, roles.ToArray());
    }
}
