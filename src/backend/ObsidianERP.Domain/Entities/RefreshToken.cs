using ObsidianERP.Domain.Common;

namespace ObsidianERP.Domain.Entities;

public class RefreshToken : BaseEntity
{
    public required string Token { get; set; }
    public required Guid UserId { get; set; }
    public required DateTime ExpiresAt { get; set; }
    public DateTime? RevokedAt { get; set; }

    public User? User { get; set; }

    public bool IsActive => RevokedAt is null && ExpiresAt > DateTime.UtcNow;
}
