using HRManagement.Api.Constants;

namespace HRManagement.Api.Entities;

public sealed class PasswordResetRequest : BaseEntity
{
    public Guid UserId { get; private set; }

    public PasswordResetStatus Status { get; private set; } = PasswordResetStatus.Pending;

    public DateTime? CompletedAtUtc { get; private set; }

    public Guid? CompletedBy { get; private set; }

    public AppUser User { get; private set; } = null!;

    private PasswordResetRequest()
    {
    }

    public PasswordResetRequest(Guid userId)
    {
        UserId = userId;
    }

    public void Complete(Guid? completedBy, DateTime completedAtUtc)
    {
        if (Status != PasswordResetStatus.Pending)
        {
            throw new InvalidOperationException(
                "Only pending password reset requests can be completed.");
        }

        Status = PasswordResetStatus.Completed;
        CompletedAtUtc = completedAtUtc;
        CompletedBy = completedBy;
    }
}