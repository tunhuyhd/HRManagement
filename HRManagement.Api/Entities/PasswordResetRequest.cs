using HRManagement.Api.Constants;

namespace HRManagement.Api.Entities;

public sealed class PasswordResetRequest : BaseEntity
{
    public Guid UserId { get; set; }

    public PasswordResetStatus Status { get; set; } = PasswordResetStatus.Pending;

    public DateTime? CompletedAtUtc { get; set; }

    public Guid? CompletedBy { get; set; }

    public AppUser User { get; set; } = null!;
}
