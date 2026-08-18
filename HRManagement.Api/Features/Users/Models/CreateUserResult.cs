namespace HRManagement.Api.Features.Users.Models;

public sealed record CreateUserResult(
    CreateUserResponse? User,
    bool EmailAlreadyExists,
    IReadOnlyCollection<string> Errors);
