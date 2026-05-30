using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ObsidianERP.Application.Abstractions;
using ObsidianERP.Application.DTOs;

namespace ObsidianERP.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request, CancellationToken cancellationToken)
        => Ok(await authService.RegisterAsync(request, cancellationToken));

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request, CancellationToken cancellationToken)
        => Ok(await authService.LoginAsync(request, cancellationToken));

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(RefreshRequest request, CancellationToken cancellationToken)
        => Ok(await authService.RefreshAsync(request.RefreshToken, cancellationToken));

    [Authorize]
    [HttpGet("me")]
    public IActionResult Me()
    {
        var id = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
        if (id is null)
        {
            return Unauthorized();
        }

        var email = User.FindFirstValue(JwtRegisteredClaimNames.Email) ?? string.Empty;
        var name = User.FindFirstValue("name") ?? string.Empty;

        return Ok(new UserDto(Guid.Parse(id), name, email));
    }
}
