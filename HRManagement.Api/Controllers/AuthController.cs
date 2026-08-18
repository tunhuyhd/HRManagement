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
    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType<LoginResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LoginResponse>> Login(LoginRequest request)
    {
        var response = await authService.LoginAsync(request);
        return response is null
            ? Problem(statusCode: StatusCodes.Status401Unauthorized, title: "Email or password is incorrect.")
            : Ok(response);
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
    [ProducesResponseType<LoginResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LoginResponse>> Refresh(RefreshTokenRequest request)
    {
        var response = await authService.RefreshAsync(request.RefreshToken);
        return response is null
            ? Problem(statusCode: StatusCodes.Status401Unauthorized, title: "Refresh token is invalid or expired.")
            : Ok(response);
    }

    [AllowAnonymous]
    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Logout(RefreshTokenRequest request)
    {
        await authService.LogoutAsync(request.RefreshToken);
        return NoContent();
    }

    [Authorize]
    [HttpPost("logout-all")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> LogoutAll()
    {
        return await authService.LogoutAllAsync() ? NoContent() : Unauthorized();
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
}
