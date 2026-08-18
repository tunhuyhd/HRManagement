using System.ComponentModel.DataAnnotations;

namespace HRManagement.Api.Features.AuditLogs.Models;

public sealed class AuditLogQuery
{
    [Range(1, int.MaxValue)]
    public int PageNumber { get; init; } = 1;

    [Range(1, 100)]
    public int PageSize { get; init; } = 20;

    [MaxLength(100)]
    public string? TableName { get; init; }

    [MaxLength(200)]
    public string? RecordId { get; init; }
}
