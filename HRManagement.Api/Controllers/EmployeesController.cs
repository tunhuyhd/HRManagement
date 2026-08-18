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

	[HttpGet]
	[ProducesResponseType<IReadOnlyList<EmployeeResponse>>(
	StatusCodes.Status200OK)]
	public async Task<ActionResult<IReadOnlyList<EmployeeResponse>>> GetList()
	{
		var employees = await employeeService.GetListAsync();

		return Ok(employees);
	}

	[HttpGet("{id:guid}")]
	[ProducesResponseType<EmployeeResponse>(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<ActionResult<EmployeeResponse>> GetById(Guid id)
	{
		var employee = await employeeService.GetByIdAsync(id);

		if (employee is null)
		{
			return NotFound(new
			{
				message = $"Employee with ID '{id}' was not found."
			});
		}

		return Ok(employee);
	}
}