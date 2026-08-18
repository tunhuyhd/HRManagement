using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HRManagement.Api.Constants;
using HRManagement.Api.Features.Users.Models;
using HRManagement.Api.Features.Users.Services;
using HRManagement.Api.Common.Pagination;

namespace HRManagement.Api.Controllers;

[ApiController]
[Authorize(Roles = AppRoles.Admin)]
[Route("api/users")]
public sealed class UsersController(IUserService userService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<PagedResponse<UserManagementResponse>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResponse<UserManagementResponse>>> GetList(
        [FromQuery] UserListQuery query)
    {
        return Ok(await userService.GetListAsync(query));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType<UserManagementResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserManagementResponse>> GetById(Guid id)
    {
        var user = await userService.GetByIdAsync(id);
        return user is null
            ? NotFound(new { message = $"User with ID '{id}' was not found." })
            : Ok(user);
    }

    [HttpPost]
    [ProducesResponseType<CreateUserResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CreateUserResponse>> Create(CreateUserRequest request)
    {
        var result = await userService.CreateAsync(request);
        if (result.EmailAlreadyExists)
        {
            return Problem(statusCode: StatusCodes.Status409Conflict, title: result.Errors.First());
        }

        if (result.User is null)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(nameof(request.Password), error);
            }

            return ValidationProblem(ModelState);
        }

        return StatusCode(StatusCodes.Status201Created, result.User);
    }

    [HttpPut("{id:guid}/access")]
    [ProducesResponseType<UserAccessResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserAccessResponse>> UpdateAccess(
        Guid id,
        UpdateUserAccessRequest request)
    {
        var result = await userService.UpdateAccessAsync(id, request);

        return result.Error switch
        {
            UpdateUserAccessError.None => Ok(result.User),
            UpdateUserAccessError.UserNotFound => NotFound(new
            {
                message = $"User with ID '{id}' was not found."
            }),
            UpdateUserAccessError.AdminUserProtected => StatusCode(
                StatusCodes.Status403Forbidden,
                new { message = "ADMIN users cannot be changed by this endpoint." }),
            _ => BadRequest(new
            {
                message = "The user could not be updated.",
                errors = result.Errors ?? Array.Empty<string>()
            })
        };
    }
}
