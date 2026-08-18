namespace HRManagement.Api.Features.Auth.Models;

public sealed record LoginResponse(
    string AccessToken,
    DateTime ExpiresAtUtc,
    UserResponse User);
