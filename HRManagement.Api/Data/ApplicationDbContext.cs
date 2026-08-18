using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using HRManagement.Api.Entities;

namespace HRManagement.Api.Data;

public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>(options) {
	public DbSet<Employee> Employees => Set<Employee>();

	protected override void OnModelCreating(ModelBuilder builder)
	{
		base.OnModelCreating(builder);

		builder.HasSequence<long>("employee_code_sequence")
		.StartsAt(1)
		.IncrementsBy(1);

		builder.ApplyConfigurationsFromAssembly(
			typeof(ApplicationDbContext).Assembly);
	}

	public override Task<int> SaveChangesAsync(
	CancellationToken cancellationToken = default)
	{
		var entries = ChangeTracker.Entries<BaseEntity>();

		foreach (var entry in entries)
		{
			if (entry.State == EntityState.Added)
			{
				entry.Entity.CreatedAtUtc = DateTime.UtcNow;
			}

			if (entry.State == EntityState.Modified)
			{
				entry.Entity.UpdatedAtUtc = DateTime.UtcNow;
			}
		}

		return base.SaveChangesAsync(cancellationToken);
	}
}
