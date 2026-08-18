using HRManagement.Api.Features.Auth.Models;

namespace HRManagement.Api.Features.Auth.Services;

public interface IAuthService
{
    Task<LoginResponse?> LoginAsync(LoginRequest request);
    Task<LoginResponse?> RefreshAsync(string refreshToken);
    Task LogoutAsync(string refreshToken);
    Task<bool> LogoutAllAsync();
    Task<UserResponse?> GetCurrentUserAsync();
}
