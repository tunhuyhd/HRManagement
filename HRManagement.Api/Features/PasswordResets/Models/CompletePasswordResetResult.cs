namespace HRManagement.Api.Features.PasswordResets.Models;

public sealed record CompletePasswordResetResult(
    PasswordResetRequestResponse? Request,
    CompletePasswordResetError Error = CompletePasswordResetError.None,
    IReadOnlyCollection<string>? Errors = null);
