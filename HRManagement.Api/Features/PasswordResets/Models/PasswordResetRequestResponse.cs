using HRManagement.Api.Constants;

namespace HRManagement.Api.Features.PasswordResets.Models;

public sealed record PasswordResetRequestResponse(
    Guid Id,
    Guid UserId,
    string Email,
    PasswordResetStatus Status,
    DateTime RequestedAtUtc,
    DateTime? CompletedAtUtc,
    Guid? CompletedBy);
