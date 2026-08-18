namespace HRManagement.Api.Entities;

public abstract class BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public Guid? LastModifiedBy { get; set; }

    public DateTime? LastModifiedAtUtc { get; set; }
}
