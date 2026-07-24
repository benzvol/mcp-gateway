using McpGateway.Core.Domain;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace McpGateway.Core.Persistence.Configurations;

public class GatewayClientConfiguration : IEntityTypeConfiguration<GatewayClient>
{
    public void Configure(EntityTypeBuilder<GatewayClient> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name).IsRequired();

        builder.Property(c => c.AuthKind).HasConversion<string>();

        builder.HasIndex(c => c.Name).IsUnique();
    }
}