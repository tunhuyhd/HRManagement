using System.Text.Json;
using HRManagement.Api.Common.Pagination;
using HRManagement.Api.Entities;
using HRManagement.Api.Features.AuditLogs.Models;
using HRManagement.Api.Repositories.Interfaces;

namespace HRManagement.Api.Features.AuditLogs.Services;

public sealed class AuditLogService(
    IAuditLogRepository auditLogRepository) : IAuditLogService
{
    public async Task<PagedResponse<AuditLogResponse>> GetListAsync(
        AuditLogQuery query)
    {
        var (auditLogs, totalCount) = await auditLogRepository.GetListAsync(
            query.PageNumber,
            query.PageSize,
            query.TableName,
            query.RecordId);

        var items = auditLogs.Select(ToResponse).ToList();
        var totalPages = (int)Math.Ceiling(totalCount / (double)query.PageSize);

        return new PagedResponse<AuditLogResponse>(
            items,
            query.PageNumber,
            query.PageSize,
            totalCount,
            totalPages);
    }

    private static AuditLogResponse ToResponse(AuditLog auditLog) => new(
        auditLog.Id,
        auditLog.TableName,
        auditLog.RecordId,
        auditLog.Action,
        ParseJson(auditLog.ChangedColumns)!.Value,
        ParseJson(auditLog.OldValues),
        ParseJson(auditLog.NewValues),
        auditLog.ChangedBy,
        auditLog.ChangedByEmail,
        auditLog.ChangedAtUtc);

    private static JsonElement? ParseJson(string? json) =>
        string.IsNullOrWhiteSpace(json)
            ? null
            : JsonSerializer.Deserialize<JsonElement>(json);
}
