namespace HRManagement.Api.Features.PasswordResets.Models;

public enum CompletePasswordResetError
{
    None,
    RequestNotFound,
    RequestAlreadyCompleted,
    UserNotFound,
    IdentityError
}
