using McpGateway.Core.Domain;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace McpGateway.Core.Persistence.Configurations;

public class ToolOverrideConfiguration : IEntityTypeConfiguration<ToolOverride>
{
    public void Configure(EntityTypeBuilder<ToolOverride> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.ToolName).IsRequired();

        builder.HasIndex(t => new { t.UpstreamId, t.ToolName }).IsUnique();

        builder.HasOne<Upstream>()
            .WithMany()
            .HasForeignKey(t => t.UpstreamId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}