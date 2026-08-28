using HRManagement.Api.Data;
using HRManagement.Api.Entities;
using HRManagement.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HRManagement.Api.Repositories;

public sealed class DepartmentRepository(
	ApplicationDbContext dbContext) : IDepartmentRepository
{
	public async Task AddAsync(Department department) =>
		await dbContext.Departments.AddAsync(department);

	public Task<Department?> GetByIdAsync(Guid id) =>
		dbContext.Departments
			.AsNoTracking()
			.FirstOrDefaultAsync(department => department.Id == id);

	public Task<bool> CodeExistsAsync(string departmentCode)
	{
		var normalizedCode =
			departmentCode.Trim().ToUpperInvariant();

		return dbContext.Departments.AnyAsync(department =>
			department.DepartmentCode == normalizedCode);
	}

	public Task SaveChangesAsync() =>
		dbContext.SaveChangesAsync();
}