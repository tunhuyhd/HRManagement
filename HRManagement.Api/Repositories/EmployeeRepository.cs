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

	public Task<Employee?> GetByIdAsync(Guid id) =>
		dbContext.Employees
			.AsNoTracking()
			.FirstOrDefaultAsync(employee => employee.Id == id);

	public Task<Employee?> GetByIdForUpdateAsync(Guid id) =>
		dbContext.Employees
			.FirstOrDefaultAsync(employee => employee.Id == id);

	public Task<bool> IsUserAssignedAsync(
		Guid userId,
		Guid? excludedEmployeeId = null) =>
		dbContext.Employees.AnyAsync(employee =>
			employee.UserId == userId &&
			(!excludedEmployeeId.HasValue || employee.Id != excludedEmployeeId.Value));

	public void Remove(Employee employee) => dbContext.Employees.Remove(employee);

	public Task SaveChangesAsync() =>
		dbContext.SaveChangesAsync();

	public async Task<(IReadOnlyList<Employee> Items, int TotalCount)>
	GetListAsync(
		int pageNumber,
		int pageSize,
		string? search)
	{
		var query = dbContext.Employees
			.AsNoTracking()
			.AsQueryable();

		if (!string.IsNullOrWhiteSpace(search))
		{
			var keyword = search.Trim();

			query = query.Where(employee =>
				EF.Functions.ILike(employee.EmployeeCode, $"%{keyword}%") ||
				EF.Functions.ILike(employee.FirstName, $"%{keyword}%") ||
				EF.Functions.ILike(employee.LastName, $"%{keyword}%") ||
				EF.Functions.ILike(
					employee.FirstName + " " + employee.LastName,
					$"%{keyword}%"));
		}

		var totalCount = await query.CountAsync();

		var employees = await query
			.OrderByDescending(employee => employee.CreatedAtUtc)
			.ThenBy(employee => employee.Id)
			.Skip((pageNumber - 1) * pageSize)
			.Take(pageSize)
			.ToListAsync();

		return (employees, totalCount);
	}
}
