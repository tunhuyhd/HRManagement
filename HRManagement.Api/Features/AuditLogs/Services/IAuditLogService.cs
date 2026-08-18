using HRManagement.Api.Common.Pagination;
using HRManagement.Api.Features.AuditLogs.Models;

namespace HRManagement.Api.Features.AuditLogs.Services;

public interface IAuditLogService
{
    Task<PagedResponse<AuditLogResponse>> GetListAsync(AuditLogQuery query);
}
