using HRManagement.Api.Data;
using HRManagement.Api.Entities;
using HRManagement.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HRManagement.Api.Repositories;

public sealed class AuditLogRepository(
    ApplicationDbContext dbContext) : IAuditLogRepository
{
    public async Task<(IReadOnlyList<AuditLog> Items, int TotalCount)> GetListAsync(
        int pageNumber,
        int pageSize,
        string? tableName,
        string? recordId)
    {
        var query = dbContext.AuditLogs.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(tableName))
        {
            var normalizedTableName = tableName.Trim().ToLowerInvariant();
            query = query.Where(auditLog => auditLog.TableName == normalizedTableName);
        }

        if (!string.IsNullOrWhiteSpace(recordId))
        {
            var normalizedRecordId = recordId.Trim();
            query = query.Where(auditLog => auditLog.RecordId == normalizedRecordId);
        }

        var totalCount = await query.CountAsync();
        var items = await query
            .OrderByDescending(auditLog => auditLog.ChangedAtUtc)
            .ThenByDescending(auditLog => auditLog.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }
}
