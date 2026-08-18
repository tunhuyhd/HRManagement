using System.ComponentModel.DataAnnotations;

namespace HRManagement.Api.Features.Users.Models;

public sealed record CreateUserRequest(
    [Required, EmailAddress] string Email,
    [Required, MinLength(6)] string Password);
