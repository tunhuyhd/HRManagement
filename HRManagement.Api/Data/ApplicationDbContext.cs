using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using HRManagement.Api.Common.Auditing;
using HRManagement.Api.Common.Auth;
using HRManagement.Api.Entities;

namespace HRManagement.Api.Data;

public sealed class ApplicationDbContext(
	DbContextOptions<ApplicationDbContext> options,
	ICurrentUser currentUser)
    : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>(options) {
	public DbSet<Employee> Employees => Set<Employee>();
	public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

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
		ChangeTracker.DetectChanges();

		var changedAtUtc = DateTime.UtcNow;
		var entries = ChangeTracker.Entries<BaseEntity>()
			.Where(entry => entry.State is
				EntityState.Added or EntityState.Modified or EntityState.Deleted)
			.ToArray();

		foreach (var entry in entries)
		{
			if (entry.State == EntityState.Added)
			{
				entry.Entity.CreatedAtUtc = changedAtUtc;
			}

			if (entry.State == EntityState.Modified)
			{
				entry.Entity.LastModifiedBy = currentUser.Id;
				entry.Entity.LastModifiedAtUtc = changedAtUtc;
			}
		}

		ChangeTracker.DetectChanges();
		AuditLogs.AddRange(AuditLogFactory.Create(entries, currentUser, changedAtUtc));

		return base.SaveChangesAsync(cancellationToken);
	}
}
