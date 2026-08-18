using HRManagement.Api.Entities;
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

		return new EmployeeResponse(
			employee.Id,
			employee.EmployeeCode,
			employee.FirstName,
			employee.LastName,
			employee.DateOfBirth,
			employee.Gender,
			employee.PhoneNumber,
			employee.Address,
			employee.HireDate,
			employee.Status,
			employee.UserId,
			employee.CreatedAtUtc);
	}
}