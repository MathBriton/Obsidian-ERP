using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ObsidianERP.Domain.Entities;

namespace ObsidianERP.Infrastructure.Persistence.Configurations;

public sealed class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("Customers");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name).IsRequired().HasMaxLength(150);
        builder.Property(c => c.Email).HasMaxLength(256);
        builder.Property(c => c.Phone).HasMaxLength(30);
        builder.Property(c => c.Document).HasMaxLength(30);

        builder.HasIndex(c => c.Name);

        // Soft delete: clientes excluídos não aparecem nas consultas.
        builder.HasQueryFilter(c => !c.IsDeleted);
    }
}
