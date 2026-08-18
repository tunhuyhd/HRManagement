namespace HRManagement.Api.Features.Users.Models;

public sealed record UpdateUserAccessResult(
    UserAccessResponse? User,
    UpdateUserAccessError Error = UpdateUserAccessError.None,
    IReadOnlyCollection<string>? Errors = null)
{
    public bool IsSuccess => Error == UpdateUserAccessError.None;
}
