using HRManagement.Api.Common.Pagination;
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
	[ProducesResponseType<PagedResponse<EmployeeResponse>>(
	StatusCodes.Status200OK)]
	public async Task<ActionResult<PagedResponse<EmployeeResponse>>> GetList(
	[FromQuery] EmployeeListQuery query)
	{
		var result = await employeeService.GetListAsync(query);

		return Ok(result);
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

	[HttpPut("{id:guid}")]
	[ProducesResponseType<EmployeeResponse>(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<ActionResult<EmployeeResponse>> Update(
		Guid id,
		UpdateEmployeeRequest request)
	{
		var employee = await employeeService.UpdateAsync(id, request);

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
