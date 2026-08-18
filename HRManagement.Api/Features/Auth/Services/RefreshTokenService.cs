using System.Security.Cryptography;
using System.Text;
using HRManagement.Api.Common.Auth;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;

namespace HRManagement.Api.Features.Auth.Services;

public sealed class RefreshTokenService(IOptions<JwtOptions> options) : IRefreshTokenService
{
    private readonly JwtOptions _options = options.Value;

    public (string Token, string Hash, DateTime ExpiresAtUtc) Create()
    {
        var token = WebEncoders.Base64UrlEncode(RandomNumberGenerator.GetBytes(64));
        var expiresAtUtc = DateTime.UtcNow.AddDays(_options.RefreshTokenExpirationDays);

        return (token, Hash(token), expiresAtUtc);
    }

    public string Hash(string token) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
}
