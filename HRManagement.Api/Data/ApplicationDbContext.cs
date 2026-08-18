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

		builder.Entity<AppUser>().ToTable("asp_net_users");
		builder.Entity<IdentityRole<Guid>>().ToTable("asp_net_roles");
		builder.Entity<IdentityUserClaim<Guid>>().ToTable("asp_net_user_claims");
		builder.Entity<IdentityUserRole<Guid>>().ToTable("asp_net_user_roles");
		builder.Entity<IdentityUserLogin<Guid>>().ToTable("asp_net_user_logins");
		builder.Entity<IdentityRoleClaim<Guid>>().ToTable("asp_net_role_claims");
		builder.Entity<IdentityUserToken<Guid>>().ToTable("asp_net_user_tokens");

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
