using HRManagement.Api.Common.Pagination;
using HRManagement.Api.Constants;
using HRManagement.Api.Features.Employees.Models;
using HRManagement.Api.Features.Employees.Services.Ipml;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRManagement.Api.Controllers;

[ApiController]
[Authorize(Roles = AppRoles.HrManager)]
[Route("api/employees")]
public sealed class EmployeesController(
	IEmployeeService employeeService) : ControllerBase
{
	[HttpPost]
	public async Task<ActionResult<EmployeeResponse>> Create(
		CreateEmployeeRequest request)
	{
		var result = await employeeService.CreateAsync(request);

		if (!result.IsSuccess)
		{
			return ToErrorResult(result.Error);
		}

		return CreatedAtAction(
			nameof(GetById),
			new { id = result.Employee!.Id },
			result.Employee);
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
		var result = await employeeService.UpdateAsync(id, request);

		if (!result.IsSuccess)
		{
			return ToErrorResult(result.Error, id);
		}

		return Ok(result.Employee);
	}

	[HttpDelete("{id:guid}")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> Delete(Guid id)
	{
		if (!await employeeService.DeleteAsync(id))
		{
			return NotFound(new
			{
				message = $"Employee with ID '{id}' was not found."
			});
		}

		return NoContent();
	}

	private ObjectResult ToErrorResult(EmployeeOperationError error, Guid? employeeId = null) =>
		error switch
		{
			EmployeeOperationError.EmployeeNotFound => NotFound(new
			{
				message = $"Employee with ID '{employeeId}' was not found."
			}),
			EmployeeOperationError.UserNotFound => BadRequest(new
			{
				message = "The selected user does not exist."
			}),
			EmployeeOperationError.UserAlreadyAssigned => Conflict(new
			{
				message = "The selected user is already assigned to another employee."
			}),
			_ => StatusCode(StatusCodes.Status500InternalServerError, new
			{
				message = "An unexpected employee operation error occurred."
			})
		};
}
