using HRManagement.Api.Constants;
using HRManagement.Api.Features.Employees.Models;
using HRManagement.Api.Features.Employees.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRManagement.Api.Controllers;

[ApiController]
[Authorize(Roles = AppRoles.Admin)]
[Route("api/employees")]
public sealed class EmployeesController(
	IEmployeeService employeeService) : ControllerBase
{
	[HttpPost]
	public async Task<ActionResult<EmployeeResponse>> Create(
		CreateEmployeeRequest request)
	{
		var employee = await employeeService.CreateAsync(request);

		if (employee is null)
		{
			return Conflict(new
			{
				message = "Employee code already exists."
			});
		}

		return StatusCode(StatusCodes.Status201Created, employee);
	}
}