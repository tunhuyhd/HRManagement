namespace HRManagement.Api.Features.PasswordResets.Models;

public sealed record PasswordOperationResult(
    bool Succeeded,
    IReadOnlyCollection<string> Errors)
{
    public static PasswordOperationResult Success() => new(true, Array.Empty<string>());

    public static PasswordOperationResult Failure(IEnumerable<string> errors) =>
        new(false, errors.ToArray());
}
