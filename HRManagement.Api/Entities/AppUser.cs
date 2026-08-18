using Microsoft.AspNetCore.Identity;

namespace HRManagement.Api.Entities;

public sealed class AppUser : IdentityUser<Guid>
{
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
