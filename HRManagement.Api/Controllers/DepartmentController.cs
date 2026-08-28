using HRManagement.Api.Constants;
using HRManagement.Api.Features.Employees.Services.Ipml;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRManagement.Api.Controllers;

[ApiController]
[Authorize(Roles = AppRoles.HrManager)]
[Route("api/departments")]
public sealed class DepartmentsController(
	IDepartmentService departmentService) : ControllerBase
{
	
}
