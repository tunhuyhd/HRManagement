using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HRManagement.Api.Features.Auth.Models;
using HRManagement.Api.Features.Auth.Services;
using HRManagement.Api.Features.PasswordResets.Models;
using HRManagement.Api.Features.PasswordResets.Services;

namespace HRManagement.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(
    IAuthService authService,
    IPasswordResetService passwordResetService) : ControllerBase
{
    private const string RefreshTokenCookieName = "hrm_refresh_token";

    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType<AuthResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
    {
        var response = await authService.LoginAsync(request);
        if (response is null)
        {
            return Problem(statusCode: StatusCodes.Status401Unauthorized, title: "Email or password is incorrect.");
        }

        SetRefreshTokenCookie(response.RefreshToken, response.RefreshTokenExpiresAtUtc);
        return Ok(ToAuthResponse(response));
    }

    [Authorize]
    [HttpPost("change-password")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ChangePassword(ChangePasswordRequest request)
    {
        var result = await passwordResetService.ChangePasswordAsync(request);
        if (result.Succeeded)
        {
            return NoContent();
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(nameof(request.NewPassword), error);
        }

        return ValidationProblem(ModelState);
    }

    [AllowAnonymous]
    [HttpPost("forgot-password")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest request)
    {
        await passwordResetService.RequestResetAsync(request);
        return Accepted(new
        {
            message = "If the account exists and is active, the request has been recorded."
        });
    }

    [AllowAnonymous]
    [HttpPost("refresh")]
    [ProducesResponseType<AuthResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponse>> Refresh()
    {
        if (!IsTrustedBrowserRequest())
        {
            return BadRequest();
        }

        if (!Request.Cookies.TryGetValue(RefreshTokenCookieName, out var refreshToken))
        {
            return Unauthorized();
        }

        var response = await authService.RefreshAsync(refreshToken);
        if (response is null)
        {
            DeleteRefreshTokenCookie();
            return Problem(statusCode: StatusCodes.Status401Unauthorized, title: "Refresh token is invalid or expired.");
        }

        SetRefreshTokenCookie(response.RefreshToken, response.RefreshTokenExpiresAtUtc);
        return Ok(ToAuthResponse(response));
    }

    [AllowAnonymous]
    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Logout()
    {
        if (!IsTrustedBrowserRequest())
        {
            return BadRequest();
        }

        if (Request.Cookies.TryGetValue(RefreshTokenCookieName, out var refreshToken))
        {
            await authService.LogoutAsync(refreshToken);
        }

        DeleteRefreshTokenCookie();
        return NoContent();
    }

    [Authorize]
    [HttpPost("logout-all")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> LogoutAll()
    {
        if (!await authService.LogoutAllAsync())
        {
            return Unauthorized();
        }

        DeleteRefreshTokenCookie();
        return NoContent();
    }

    [Authorize]
    [HttpGet("me")]
    [ProducesResponseType<UserResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UserResponse>> Me()
    {
        var response = await authService.GetCurrentUserAsync();
        return response is null ? Unauthorized() : Ok(response);
    }

    private static AuthResponse ToAuthResponse(LoginResponse response) =>
        new(response.AccessToken, response.ExpiresAtUtc, response.User);

    private void SetRefreshTokenCookie(string refreshToken, DateTime expiresAtUtc) =>
        Response.Cookies.Append(
            RefreshTokenCookieName,
            refreshToken,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = expiresAtUtc,
                Path = "/api/auth"
            });

    private void DeleteRefreshTokenCookie() =>
        Response.Cookies.Delete(
            RefreshTokenCookieName,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Path = "/api/auth"
            });

    private bool IsTrustedBrowserRequest() =>
        Request.Headers["X-Requested-With"] == "XMLHttpRequest";
}
