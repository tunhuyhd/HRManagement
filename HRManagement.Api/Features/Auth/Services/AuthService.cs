using HRManagement.Api.Common.Auth;
using HRManagement.Api.Features.Auth.Models;
using HRManagement.Api.Entities;
using HRManagement.Api.Repositories.Interfaces;

namespace HRManagement.Api.Features.Auth.Services;

public sealed class AuthService(
    IUserRepository userRepository,
    IRefreshTokenRepository refreshTokenRepository,
    IJwtTokenService jwtTokenService,
    IRefreshTokenService refreshTokenService,
    ICurrentUser currentUser) : IAuthService
{
    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        var user = await userRepository.FindByEmailAsync(request.Email.Trim());
        if (user is null || !await userRepository.CheckPasswordAsync(user, request.Password))
        {
            return null;
        }

        return await CreateSessionAsync(user);
    }

    public async Task<LoginResponse?> RefreshAsync(string refreshToken)
    {
        var tokenHash = refreshTokenService.Hash(refreshToken);
        var storedToken = await refreshTokenRepository.GetByHashForUpdateAsync(tokenHash);
        var nowUtc = DateTime.UtcNow;

        if (storedToken is null ||
            storedToken.RevokedAtUtc.HasValue ||
            storedToken.ExpiresAtUtc <= nowUtc)
        {
            return null;
        }

        var user = await userRepository.FindByIdAsync(storedToken.UserId);
        if (user is null || user.LockoutEnd > DateTimeOffset.UtcNow)
        {
            return null;
        }

        return await CreateSessionAsync(user, storedToken, nowUtc);
    }

    public async Task LogoutAsync(string refreshToken)
    {
        var tokenHash = refreshTokenService.Hash(refreshToken);
        var storedToken = await refreshTokenRepository.GetByHashForUpdateAsync(tokenHash);

        if (storedToken is null || storedToken.RevokedAtUtc.HasValue)
        {
            return;
        }

        storedToken.RevokedAtUtc = DateTime.UtcNow;
        await refreshTokenRepository.SaveChangesAsync();
    }

    public async Task<bool> LogoutAllAsync()
    {
        if (currentUser.Id is not { } userId)
        {
            return false;
        }

        var nowUtc = DateTime.UtcNow;
        var activeTokens = await refreshTokenRepository.GetActiveByUserIdAsync(userId, nowUtc);

        foreach (var token in activeTokens)
        {
            token.RevokedAtUtc = nowUtc;
        }

        await refreshTokenRepository.SaveChangesAsync();
        return true;
    }

    public async Task<UserResponse?> GetCurrentUserAsync()
    {
        if (currentUser.Id is not { } userId || currentUser.Email is null)
        {
            return null;
        }

        var user = await userRepository.FindByEmailAsync(currentUser.Email);
        if (user is null || user.Id != userId)
        {
            return null;
        }

        var roles = await userRepository.GetRolesAsync(user);
        return new UserResponse(user.Id, user.Email!, roles.ToArray());
    }

    private async Task<LoginResponse> CreateSessionAsync(
        AppUser user,
        RefreshToken? tokenToReplace = null,
        DateTime? revokedAtUtc = null)
    {
        var roles = await userRepository.GetRolesAsync(user);
        var (accessToken, accessTokenExpiresAtUtc) = jwtTokenService.Create(user, roles);
        var (refreshToken, refreshTokenHash, refreshTokenExpiresAtUtc) =
            refreshTokenService.Create();

        var newRefreshToken = new RefreshToken
        {
            UserId = user.Id,
            TokenHash = refreshTokenHash,
            ExpiresAtUtc = refreshTokenExpiresAtUtc
        };

        await refreshTokenRepository.AddAsync(newRefreshToken);

        if (tokenToReplace is not null)
        {
            tokenToReplace.RevokedAtUtc = revokedAtUtc ?? DateTime.UtcNow;
            tokenToReplace.ReplacedByTokenId = newRefreshToken.Id;
        }

        await refreshTokenRepository.SaveChangesAsync();

        return new LoginResponse(
            accessToken,
            accessTokenExpiresAtUtc,
            refreshToken,
            refreshTokenExpiresAtUtc,
            new UserResponse(user.Id, user.Email!, roles.ToArray()));
    }
}
