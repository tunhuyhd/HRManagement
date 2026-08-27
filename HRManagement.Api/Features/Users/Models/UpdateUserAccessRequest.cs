namespace HRManagement.Api.Features.Users.Models;

public sealed record UpdateUserAccessRequest(
    Guid RoleId,
    bool IsActive);
