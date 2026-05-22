using ObsidianERP.Domain.Entities;

namespace ObsidianERP.Application.Abstractions;

public sealed record AccessToken(string Value, DateTime ExpiresAt);

public interface IJwtTokenGenerator
{
    AccessToken GenerateAccessToken(User user);
    string GenerateRefreshToken();
}
