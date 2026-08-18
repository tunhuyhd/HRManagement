using System.Text.Json;

namespace HRManagement.Api.Features.AuditLogs.Models;

public sealed record AuditLogResponse(
    Guid Id,
    string TableName,
    string RecordId,
    string Action,
    JsonElement ChangedColumns,
    JsonElement? OldValues,
    JsonElement? NewValues,
    Guid? ChangedBy,
    string? ChangedByEmail,
    DateTime ChangedAtUtc);
