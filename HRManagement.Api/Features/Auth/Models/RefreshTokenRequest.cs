using System.ComponentModel.DataAnnotations;

namespace HRManagement.Api.Features.Auth.Models;

public sealed record RefreshTokenRequest(
    [Required, MaxLength(500)] string RefreshToken);
