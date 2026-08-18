using HRManagement.Api.Entities;

namespace HRManagement.Api.Features.Auth.Services;

public interface IJwtTokenService
{
    (string Token, DateTime ExpiresAtUtc) Create(AppUser user, IEnumerable<string> roles);
}
