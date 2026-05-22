using ObsidianERP.Domain.Common;

namespace ObsidianERP.Domain.Entities;

public class User : BaseEntity
{
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }

    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}
