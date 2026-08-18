namespace HRManagement.Api.Entities;

public sealed class AuditLog
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public required string TableName { get; set; }

    public required string RecordId { get; set; }

    public required string Action { get; set; }

    public required string ChangedColumns { get; set; }

    public string? OldValues { get; set; }

    public string? NewValues { get; set; }

    public Guid? ChangedBy { get; set; }

    public string? ChangedByEmail { get; set; }

    public DateTime ChangedAtUtc { get; set; }
}
