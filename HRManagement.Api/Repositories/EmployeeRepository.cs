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

	public Task SaveChangesAsync() =>
		dbContext.SaveChangesAsync();
}