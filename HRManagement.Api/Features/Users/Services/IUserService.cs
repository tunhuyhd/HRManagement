using HRManagement.Api.Features.Users.Models;

namespace HRManagement.Api.Features.Users.Services;

public interface IUserService
{
    Task<CreateUserResult> CreateAsync(CreateUserRequest request);
    Task<UpdateUserAccessResult> UpdateAccessAsync(Guid id, UpdateUserAccessRequest request);
}
