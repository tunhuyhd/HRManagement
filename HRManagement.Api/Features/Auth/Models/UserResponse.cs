namespace HRManagement.Api.Features.Auth.Models;

public sealed record UserResponse(
    Guid Id,
    string Email,
    IReadOnlyCollection<string> Roles);
