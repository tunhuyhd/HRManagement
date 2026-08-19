namespace HRManagement.Api.Features.Auth.Models;

public sealed record AuthResponse(
    string AccessToken,
    DateTime ExpiresAtUtc,
    UserResponse User);
