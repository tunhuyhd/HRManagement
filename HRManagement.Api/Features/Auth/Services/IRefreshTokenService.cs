namespace HRManagement.Api.Features.Auth.Services;

public interface IRefreshTokenService
{
    (string Token, string Hash, DateTime ExpiresAtUtc) Create();
    string Hash(string token);
}
