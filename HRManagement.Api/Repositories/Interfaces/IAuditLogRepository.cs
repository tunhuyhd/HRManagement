using HRManagement.Api.Entities;

namespace HRManagement.Api.Repositories.Interfaces;

public interface IAuditLogRepository
{
    Task<(IReadOnlyList<AuditLog> Items, int TotalCount)> GetListAsync(
        int pageNumber,
        int pageSize,
        string? tableName,
        string? recordId);
}
