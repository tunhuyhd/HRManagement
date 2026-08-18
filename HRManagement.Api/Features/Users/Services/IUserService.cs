using HRManagement.Api.Features.Users.Models;
using HRManagement.Api.Common.Pagination;

namespace HRManagement.Api.Features.Users.Services;

public interface IUserService
{
    Task<CreateUserResult> CreateAsync(CreateUserRequest request);
    Task<PagedResponse<UserManagementResponse>> GetListAsync(UserListQuery query);
    Task<UserManagementResponse?> GetByIdAsync(Guid id);
    Task<UpdateUserAccessResult> UpdateAccessAsync(Guid id, UpdateUserAccessRequest request);
}
