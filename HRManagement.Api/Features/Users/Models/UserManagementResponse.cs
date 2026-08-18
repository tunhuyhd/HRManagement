namespace HRManagement.Api.Features.Users.Models;

public sealed record UserManagementResponse(
    Guid Id,
    string Email,
    IReadOnlyCollection<string> Roles,
    bool IsActive,
    DateTime CreatedAtUtc);
