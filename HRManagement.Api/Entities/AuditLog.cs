namespace HRManagement.Api.Entities;

public sealed class AuditLog
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    public string TableName { get; private set; } = null!;

    public string RecordId { get; private set; } = null!;

    public string Action { get; private set; } = null!;

    public string ChangedColumns { get; private set; } = null!;

    public string? OldValues { get; private set; }

    public string? NewValues { get; private set; }

    public Guid? ChangedBy { get; private set; }

    public string? ChangedByEmail { get; private set; }

    public DateTime ChangedAtUtc { get; private set; }

    private AuditLog()
    {
    }

    public AuditLog(
        string tableName,
        string recordId,
        string action,
        string changedColumns,
        string? oldValues,
        string? newValues,
        Guid? changedBy,
        string? changedByEmail,
        DateTime changedAtUtc)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(tableName);
        ArgumentException.ThrowIfNullOrWhiteSpace(recordId);
        ArgumentException.ThrowIfNullOrWhiteSpace(action);
        ArgumentException.ThrowIfNullOrWhiteSpace(changedColumns);

        TableName = tableName;
        RecordId = recordId;
        Action = action;
        ChangedColumns = changedColumns;
        OldValues = oldValues;
        NewValues = newValues;
        ChangedBy = changedBy;
        ChangedByEmail = changedByEmail;
        ChangedAtUtc = changedAtUtc;
    }
}