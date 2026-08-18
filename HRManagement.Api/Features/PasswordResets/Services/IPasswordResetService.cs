using HRManagement.Api.Common.Pagination;
using HRManagement.Api.Features.PasswordResets.Models;

namespace HRManagement.Api.Features.PasswordResets.Services;

public interface IPasswordResetService
{
    Task<PasswordOperationResult> ChangePasswordAsync(ChangePasswordRequest request);
    Task RequestResetAsync(ForgotPasswordRequest request);
    Task<PagedResponse<PasswordResetRequestResponse>> GetListAsync(
        PasswordResetRequestQuery query);
    Task<PasswordResetRequestResponse?> GetByIdAsync(Guid id);
    Task<CompletePasswordResetResult> CompleteAsync(
        Guid id,
        CompletePasswordResetRequest request);
}
