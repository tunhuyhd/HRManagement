using HRManagement.Api.Common.Pagination;
using HRManagement.Api.Entities;
using HRManagement.Api.Features.Employees.Mappings;
using HRManagement.Api.Features.Employees.Models;
using HRManagement.Api.Repositories.Interfaces;

namespace HRManagement.Api.Features.Employees.Services;

public sealed class EmployeeService(
	IEmployeeRepository employeeRepository) : IEmployeeService
{
	public async Task<EmployeeResponse?> CreateAsync(
		CreateEmployeeRequest request)
	{
		var employee = new Employee
		{
			FirstName = request.FirstName.Trim(),
			LastName = request.LastName.Trim(),
			DateOfBirth = request.DateOfBirth,
			Gender = request.Gender,
			PhoneNumber = request.PhoneNumber?.Trim(),
			Address = request.Address?.Trim(),
			HireDate = request.HireDate,
			UserId = request.UserId
		};

		await employeeRepository.AddAsync(employee);
		await employeeRepository.SaveChangesAsync();

		return employee.ToResponse();
	}

	public async Task<IReadOnlyList<EmployeeResponse>> GetListAsync()
	{
		var employees = await employeeRepository.GetListAsync();

		return employees
			.Select(employee => employee.ToResponse())
			.ToList();
	}

	public async Task<EmployeeResponse?> GetByIdAsync(Guid id)
	{
		var employee = await employeeRepository.GetByIdAsync(id);

		return employee?.ToResponse();
	}

	public async Task<EmployeeResponse?> UpdateAsync(
		Guid id,
		UpdateEmployeeRequest request)
	{
		var employee = await employeeRepository.GetByIdForUpdateAsync(id);
		if (employee is null)
		{
			return null;
		}

		employee.FirstName = request.FirstName.Trim();
		employee.LastName = request.LastName.Trim();
		employee.DateOfBirth = request.DateOfBirth;
		employee.Gender = request.Gender;
		employee.PhoneNumber = request.PhoneNumber?.Trim();
		employee.Address = request.Address?.Trim();
		employee.HireDate = request.HireDate;
		employee.Status = request.Status;
		employee.UserId = request.UserId;

		await employeeRepository.SaveChangesAsync();

		return employee.ToResponse();
	}

	public async Task<PagedResponse<EmployeeResponse>> GetListAsync(
	EmployeeListQuery query)
	{
		var (employees, totalCount) =
			await employeeRepository.GetListAsync(
				query.PageNumber,
				query.PageSize,
				query.Search);

		var items = employees
			.Select(employee => employee.ToResponse())
			.ToList();

		var totalPages = (int)Math.Ceiling(
			totalCount / (double)query.PageSize);

		return new PagedResponse<EmployeeResponse>(
			items,
			query.PageNumber,
			query.PageSize,
			totalCount,
			totalPages);
	}
}
