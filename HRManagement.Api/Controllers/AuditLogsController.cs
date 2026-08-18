using HRManagement.Api.Common.Pagination;
using HRManagement.Api.Constants;
using HRManagement.Api.Features.AuditLogs.Models;
using HRManagement.Api.Features.AuditLogs.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRManagement.Api.Controllers;

[ApiController]
[Authorize(Roles = AppRoles.HrManager)]
[Route("api/audit-logs")]
public sealed class AuditLogsController(
    IAuditLogService auditLogService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<PagedResponse<AuditLogResponse>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResponse<AuditLogResponse>>> GetList(
        [FromQuery] AuditLogQuery query)
    {
        var result = await auditLogService.GetListAsync(query);
        return Ok(result);
    }
}
