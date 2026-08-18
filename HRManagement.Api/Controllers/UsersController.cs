using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HRManagement.Api.Constants;
using HRManagement.Api.Features.Users.Models;
using HRManagement.Api.Features.Users.Services;

namespace HRManagement.Api.Controllers;

[ApiController]
[Authorize(Roles = AppRoles.Admin)]
[Route("api/users")]
public sealed class UsersController(IUserService userService) : ControllerBase
{
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
}
