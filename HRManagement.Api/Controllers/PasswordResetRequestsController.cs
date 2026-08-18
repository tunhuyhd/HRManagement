using HRManagement.Api.Common.Pagination;
using HRManagement.Api.Constants;
using HRManagement.Api.Features.PasswordResets.Models;
using HRManagement.Api.Features.PasswordResets.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRManagement.Api.Controllers;

[ApiController]
[Authorize(Roles = AppRoles.Admin)]
[Route("api/password-reset-requests")]
public sealed class PasswordResetRequestsController(
    IPasswordResetService passwordResetService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<PagedResponse<PasswordResetRequestResponse>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResponse<PasswordResetRequestResponse>>> GetList(
        [FromQuery] PasswordResetRequestQuery query) =>
        Ok(await passwordResetService.GetListAsync(query));

    [HttpGet("{id:guid}")]
    [ProducesResponseType<PasswordResetRequestResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PasswordResetRequestResponse>> GetById(Guid id)
    {
        var request = await passwordResetService.GetByIdAsync(id);
        return request is null
            ? NotFound(new { message = $"Password reset request with ID '{id}' was not found." })
            : Ok(request);
    }

    [HttpPut("{id:guid}/password")]
    [ProducesResponseType<PasswordResetRequestResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<PasswordResetRequestResponse>> Complete(
        Guid id,
        CompletePasswordResetRequest request)
    {
        var result = await passwordResetService.CompleteAsync(id, request);

        return result.Error switch
        {
            CompletePasswordResetError.None => Ok(result.Request),
            CompletePasswordResetError.RequestNotFound or
                CompletePasswordResetError.UserNotFound => NotFound(new
                {
                    message = "The password reset request or its user was not found."
                }),
            CompletePasswordResetError.RequestAlreadyCompleted => Conflict(new
                {
                    message = "The password reset request has already been completed."
                }),
            _ => BadRequest(new
                {
                    message = "The password could not be reset.",
                    errors = result.Errors ?? Array.Empty<string>()
                })
        };
    }
}
