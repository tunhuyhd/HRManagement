namespace HRManagement.Api.Features.Users.Models;

public sealed record UserAccessResponse(
    Guid Id,
    string Email,
    string Role,
    bool IsActive);
