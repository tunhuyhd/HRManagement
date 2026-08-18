namespace HRManagement.Api.Features.Users.Models;

public enum UpdateUserAccessError
{
    None,
    UserNotFound,
    AdminUserProtected,
    IdentityError
}
