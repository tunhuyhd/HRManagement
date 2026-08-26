using HRManagement.Api.Common.Pagination;
using HRManagement.Api.Entities;
using HRManagement.Api.Features.Employees.Mappings;
using HRManagement.Api.Features.Employees.Models;
using HRManagement.Api.Repositories.Interfaces;

namespace HRManagement.Api.Features.Employees.Services;

public sealed class EmployeeService(
	IEmployeeRepository employeeRepository,
	IUserRepository userRepository) : IEmployeeService
{
	public async Task<EmployeeOperationResult> CreateAsync(
		CreateEmployeeRequest request)
	{
		var userError = await ValidateUserAssignmentAsync(request.UserId);
		if (userError != EmployeeOperationError.None)
		{
			return EmployeeOperationResult.Failure(userError);
		}

		var employee = new Employee(
			request.FirstName,
			request.LastName,
			request.DateOfBirth,
			request.Gender,
			request.PhoneNumber,
			request.Address,
			request.HireDate,
			request.UserId,
			request.Email);

		await employeeRepository.AddAsync(employee);
		await employeeRepository.SaveChangesAsync();

		return EmployeeOperationResult.Success(employee.ToResponse());
	}

	public async Task<EmployeeResponse?> GetByIdAsync(Guid id)
	{
		var employee = await employeeRepository.GetByIdAsync(id);

		return employee?.ToResponse();
	}

	public async Task<EmployeeOperationResult> UpdateAsync(
		Guid id,
		UpdateEmployeeRequest request)
	{
		var employee = await employeeRepository.GetByIdForUpdateAsync(id);
		if (employee is null)
		{
			return EmployeeOperationResult.Failure(EmployeeOperationError.EmployeeNotFound);
		}

		var userError = await ValidateUserAssignmentAsync(request.UserId, id);
		if (userError != EmployeeOperationError.None)
		{
			return EmployeeOperationResult.Failure(userError);
		}

		employee.Update(
			request.FirstName,
			request.LastName,
			request.DateOfBirth,
			request.Gender,
			request.PhoneNumber ?? String.Empty,
			request.Address ?? string.Empty,
			request.HireDate,
			request.Status,
			request.UserId,
			request.Email);

		await employeeRepository.SaveChangesAsync();

		return EmployeeOperationResult.Success(employee.ToResponse());
	}

	public async Task<bool> DeleteAsync(Guid id)
	{
		var employee = await employeeRepository.GetByIdForUpdateAsync(id);
		if (employee is null)
		{
			return false;
		}

		employeeRepository.Remove(employee);
		await employeeRepository.SaveChangesAsync();

		return true;
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

	private async Task<EmployeeOperationError> ValidateUserAssignmentAsync(
		Guid? userId,
		Guid? excludedEmployeeId = null)
	{
		if (!userId.HasValue)
		{
			return EmployeeOperationError.None;
		}

		if (await userRepository.FindByIdAsync(userId.Value) is null)
		{
			return EmployeeOperationError.UserNotFound;
		}

		return await employeeRepository.IsUserAssignedAsync(userId.Value, excludedEmployeeId)
			? EmployeeOperationError.UserAlreadyAssigned
			: EmployeeOperationError.None;
	}
}
