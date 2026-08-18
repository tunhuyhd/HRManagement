namespace HRManagement.Api.Features.Users.Models;

public sealed record CreateUserResponse(Guid Id, string Email, string Role);
