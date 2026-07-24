using McpGateway.Core.Domain;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace McpGateway.Core.Persistence.Configurations;

public class AuditEntryConfiguration : IEntityTypeConfiguration<AuditEntry>
{
    public void Configure(EntityTypeBuilder<AuditEntry> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.ToolName).IsRequired();

        builder.Property(a => a.Status).HasConversion<string>();

        builder.HasIndex(a => new { a.Timestamp, a.ClientName, a.ToolName });
    }
}