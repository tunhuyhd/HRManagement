namespace HRManagement.Api.Entities;

public abstract class BaseEntity
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    public DateTime CreatedAtUtc { get; internal set; } = DateTime.UtcNow;

    public Guid? LastModifiedBy { get; internal set; }

    public DateTime? LastModifiedAtUtc { get; internal set; }

    public bool IsDeleted { get; internal set; }

    public Guid? DeletedBy { get; internal set; }

    public DateTime? DeletedAtUtc { get; internal set; }
}