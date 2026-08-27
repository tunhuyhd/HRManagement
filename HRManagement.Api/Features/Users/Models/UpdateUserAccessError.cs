namespace HRManagement.Api.Features.Users.Models;

public enum UpdateUserAccessError
{
    None,
    UserNotFound,
    RoleNotFound,
    RoleNotAssignable,
    AdminUserProtected,
    IdentityError
}
