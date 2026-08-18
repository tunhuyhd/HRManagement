using HRManagement.Api.Common.Auth;
using HRManagement.Api.Common.Pagination;
using HRManagement.Api.Constants;
using HRManagement.Api.Entities;
using HRManagement.Api.Features.PasswordResets.Models;
using HRManagement.Api.Repositories.Interfaces;

namespace HRManagement.Api.Features.PasswordResets.Services;

public sealed class PasswordResetService(
    IUserRepository userRepository,
    IPasswordResetRequestRepository passwordResetRequestRepository,
    IRefreshTokenRepository refreshTokenRepository,
    ICurrentUser currentUser) : IPasswordResetService
{
    public async Task<PasswordOperationResult> ChangePasswordAsync(
        ChangePasswordRequest request)
    {
        if (currentUser.Id is not { } userId)
        {
            return PasswordOperationResult.Failure(["The current user was not found."]);
        }

        var user = await userRepository.FindByIdAsync(userId);
        if (user is null || !user.IsActive)
        {
            return PasswordOperationResult.Failure(["The current user is inactive or no longer exists."]);
        }

        var result = await userRepository.ChangePasswordAsync(
            user,
            request.CurrentPassword,
            request.NewPassword);

        if (!result.Succeeded)
        {
            return PasswordOperationResult.Failure(
                result.Errors.Select(error => error.Description));
        }

        await RevokeRefreshTokensAsync(user.Id);
        return PasswordOperationResult.Success();
    }

    public async Task RequestResetAsync(ForgotPasswordRequest request)
    {
        var user = await userRepository.FindByEmailAsync(request.Email.Trim());
        if (user is null ||
            !user.IsActive ||
            await passwordResetRequestRepository.HasPendingRequestAsync(user.Id))
        {
            return;
        }

        await passwordResetRequestRepository.AddAsync(new PasswordResetRequest
        {
            UserId = user.Id
        });
        await passwordResetRequestRepository.SaveChangesAsync();
    }

    public async Task<PagedResponse<PasswordResetRequestResponse>> GetListAsync(
        PasswordResetRequestQuery query)
    {
        var (requests, totalCount) = await passwordResetRequestRepository.GetListAsync(
            query.PageNumber,
            query.PageSize,
            query.Search,
            query.Status);

        return new PagedResponse<PasswordResetRequestResponse>(
            requests.Select(ToResponse).ToList(),
            query.PageNumber,
            query.PageSize,
            totalCount,
            (int)Math.Ceiling(totalCount / (double)query.PageSize));
    }

    public async Task<PasswordResetRequestResponse?> GetByIdAsync(Guid id)
    {
        var request = await passwordResetRequestRepository.GetByIdAsync(id);
        return request is null ? null : ToResponse(request);
    }

    public async Task<CompletePasswordResetResult> CompleteAsync(
        Guid id,
        CompletePasswordResetRequest request)
    {
        var resetRequest = await passwordResetRequestRepository.GetByIdForUpdateAsync(id);
        if (resetRequest is null)
        {
            return new CompletePasswordResetResult(
                null,
                CompletePasswordResetError.RequestNotFound);
        }

        if (resetRequest.Status != PasswordResetStatus.Pending)
        {
            return new CompletePasswordResetResult(
                null,
                CompletePasswordResetError.RequestAlreadyCompleted);
        }

        var user = await userRepository.FindByIdAsync(resetRequest.UserId);
        if (user is null)
        {
            return new CompletePasswordResetResult(
                null,
                CompletePasswordResetError.UserNotFound);
        }

        var identityResult = await userRepository.ResetPasswordAsync(user, request.NewPassword);
        if (!identityResult.Succeeded)
        {
            return new CompletePasswordResetResult(
                null,
                CompletePasswordResetError.IdentityError,
                identityResult.Errors.Select(error => error.Description).ToArray());
        }

        var nowUtc = DateTime.UtcNow;
        resetRequest.Status = PasswordResetStatus.Completed;
        resetRequest.CompletedAtUtc = nowUtc;
        resetRequest.CompletedBy = currentUser.Id;

        await RevokeRefreshTokensAsync(user.Id, nowUtc);
        await passwordResetRequestRepository.SaveChangesAsync();

        return new CompletePasswordResetResult(ToResponse(resetRequest));
    }

    private async Task RevokeRefreshTokensAsync(Guid userId, DateTime? revokedAtUtc = null)
    {
        var nowUtc = revokedAtUtc ?? DateTime.UtcNow;
        var activeTokens = await refreshTokenRepository.GetActiveByUserIdAsync(userId, nowUtc);

        foreach (var token in activeTokens)
        {
            token.RevokedAtUtc = nowUtc;
        }

        await refreshTokenRepository.SaveChangesAsync();
    }

    private static PasswordResetRequestResponse ToResponse(PasswordResetRequest request) =>
        new(
            request.Id,
            request.UserId,
            request.User.Email!,
            request.Status,
            request.CreatedAtUtc,
            request.CompletedAtUtc,
            request.CompletedBy);
}
