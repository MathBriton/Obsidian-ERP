using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using ObsidianERP.Application.Abstractions;
using ObsidianERP.Application.DTOs;
using ObsidianERP.Application.Exceptions;

namespace ObsidianERP.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IValidator<RegisterRequest> _registerValidator;
    private readonly IValidator<LoginRequest> _loginValidator;

    public AuthController(
        IAuthService authService,
        IValidator<RegisterRequest> registerValidator,
        IValidator<LoginRequest> loginValidator)
    {
        _authService = authService;
        _registerValidator = registerValidator;
        _loginValidator = loginValidator;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request, CancellationToken cancellationToken)
    {
        var validation = await _registerValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return ValidationProblem(ToModelState(validation));
        }

        try
        {
            return Ok(await _authService.RegisterAsync(request, cancellationToken));
        }
        catch (EmailAlreadyInUseException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        var validation = await _loginValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return ValidationProblem(ToModelState(validation));
        }

        try
        {
            return Ok(await _authService.LoginAsync(request, cancellationToken));
        }
        catch (InvalidCredentialsException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(RefreshRequest request, CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await _authService.RefreshAsync(request.RefreshToken, cancellationToken));
        }
        catch (InvalidRefreshTokenException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }

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

    private ModelStateDictionary ToModelState(ValidationResult validation)
    {
        foreach (var error in validation.Errors)
        {
            ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
        }

        return ModelState;
    }
}
