using Microsoft.AspNetCore.Identity;

namespace HRManagement.Api.Entities;

public sealed class AppUser : IdentityUser<Guid>
{
    public DateTime CreatedAtUtc { get; private set; } = DateTime.UtcNow;

    public bool IsActive { get; private set; } = true;

    public void SetActive(bool isActive) => IsActive = isActive;
}