using ObsidianERP.Application.Abstractions;
using ObsidianERP.Application.Common;
using ObsidianERP.Application.DTOs;
using ObsidianERP.Application.Exceptions;
using ObsidianERP.Domain.Entities;

namespace ObsidianERP.Application.Services;

public sealed class AuthService : IAuthService
{
    private readonly IUserRepository _users;
    private readonly IRefreshTokenRepository _refreshTokens;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _tokenGenerator;
    private readonly JwtSettings _jwtSettings;

    public AuthService(
        IUserRepository users,
        IRefreshTokenRepository refreshTokens,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator tokenGenerator,
        JwtSettings jwtSettings)
    {
        _users = users;
        _refreshTokens = refreshTokens;
        _passwordHasher = passwordHasher;
        _tokenGenerator = tokenGenerator;
        _jwtSettings = jwtSettings;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var email = Normalize(request.Email);

        if (await _users.EmailExistsAsync(email, cancellationToken))
        {
            throw new EmailAlreadyInUseException(email);
        }

        var user = new User
        {
            Name = request.Name.Trim(),
            Email = email,
            PasswordHash = _passwordHasher.Hash(request.Password),
        };

        await _users.AddAsync(user, cancellationToken);
        await _users.SaveChangesAsync(cancellationToken);

        return await IssueTokensAsync(user, cancellationToken);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _users.GetByEmailAsync(Normalize(request.Email), cancellationToken);

        if (user is null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            throw new InvalidCredentialsException();
        }

        return await IssueTokensAsync(user, cancellationToken);
    }

    public async Task<AuthResponse> RefreshAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        var stored = await _refreshTokens.GetByTokenAsync(refreshToken, cancellationToken);

        if (stored is null || !stored.IsActive)
        {
            throw new InvalidRefreshTokenException();
        }

        var user = stored.User
            ?? await _users.GetByIdAsync(stored.UserId, cancellationToken)
            ?? throw new InvalidRefreshTokenException();

        // Rotação: revoga o token usado antes de emitir um novo.
        stored.RevokedAt = DateTime.UtcNow;

        return await IssueTokensAsync(user, cancellationToken);
    }

    private async Task<AuthResponse> IssueTokensAsync(User user, CancellationToken cancellationToken)
    {
        var accessToken = _tokenGenerator.GenerateAccessToken(user);
        var refreshToken = _tokenGenerator.GenerateRefreshToken();

        await _refreshTokens.AddAsync(
            new RefreshToken
            {
                Token = refreshToken,
                UserId = user.Id,
                ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenDays),
            },
            cancellationToken);
        await _refreshTokens.SaveChangesAsync(cancellationToken);

        return new AuthResponse(
            accessToken.Value,
            refreshToken,
            accessToken.ExpiresAt,
            new UserDto(user.Id, user.Name, user.Email));
    }

    private static string Normalize(string email) => email.Trim().ToLowerInvariant();
}
