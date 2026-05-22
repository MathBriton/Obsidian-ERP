using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ObsidianERP.Domain.Entities;

namespace ObsidianERP.Infrastructure.Persistence.Configurations;

public sealed class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("RefreshTokens");
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Token).IsRequired().HasMaxLength(512);
        builder.HasIndex(r => r.Token).IsUnique();

        builder.Ignore(r => r.IsActive);
    }
}
