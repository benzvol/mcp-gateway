using McpGateway.Core.Domain;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace McpGateway.Core.Persistence.Configurations;

public class UpstreamConfiguration : IEntityTypeConfiguration<Upstream>
{
    public void Configure(EntityTypeBuilder<Upstream> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Name).IsRequired();

        builder.Property(u => u.Transport).HasConversion<string>();

        builder.Property(u => u.Args)
            .HasConversion(JsonValueConverters.StringList)
            .Metadata.SetValueComparer(JsonValueConverters.StringListComparer);

        builder.Property(u => u.Environment)
            .HasConversion(JsonValueConverters.StringDictionary)
            .Metadata.SetValueComparer(JsonValueConverters.StringDictionaryComparer);

        builder.Property(u => u.Headers)
            .HasConversion(JsonValueConverters.StringDictionary)
            .Metadata.SetValueComparer(JsonValueConverters.StringDictionaryComparer);

        builder.Property(u => u.AuthKind).HasConversion<string>();

        builder.HasIndex(u => u.Name).IsUnique();
    }
}