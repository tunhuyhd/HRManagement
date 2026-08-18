using HRManagement.Api.Data;
using HRManagement.Api.Entities;
using HRManagement.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HRManagement.Api.Repositories;

public sealed class EmployeeRepository(
	ApplicationDbContext dbContext) : IEmployeeRepository
{
	public async Task AddAsync(Employee employee) =>
	await dbContext.Employees.AddAsync(employee);

	public async Task<IReadOnlyList<Employee>> GetListAsync() =>
	await dbContext.Employees
		.AsNoTracking()
		.OrderByDescending(employee => employee.CreatedAtUtc)
		.ToListAsync();

	public Task<Employee?> GetByIdAsync(Guid id) =>
		dbContext.Employees
			.AsNoTracking()
			.FirstOrDefaultAsync(employee => employee.Id == id);

	public Task SaveChangesAsync() =>
		dbContext.SaveChangesAsync();
}