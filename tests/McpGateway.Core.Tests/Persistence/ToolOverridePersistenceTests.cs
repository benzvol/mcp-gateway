using McpGateway.Core.Domain;
using McpGateway.TestUtils.Persistence;

using Microsoft.EntityFrameworkCore;

namespace McpGateway.Core.Tests.Persistence;

public class ToolOverridePersistenceTests : PersistenceTestBase
{
    [Test]
    public async Task ToolOverride_RoundTripsOverrideFields()
    {
        var upstream = new Upstream { Id = Guid.NewGuid(), Name = "upstream", Transport = TransportKind.Stdio };
        Context.Upstreams.Add(upstream);
        await Context.SaveChangesAsync();

        var toolOverride = new ToolOverride
        {
            Id = Guid.NewGuid(),
            UpstreamId = upstream.Id,
            ToolName = "search",
            Enabled = false,
            OverrideName = "web-search",
            OverrideDescription = "Search the web",
        };
        Context.ToolOverrides.Add(toolOverride);
        await Context.SaveChangesAsync();

        await using var freshContext = CreateContext();
        var loaded = await freshContext.ToolOverrides.SingleAsync(t => t.Id == toolOverride.Id);

        await Assert.That(loaded.UpstreamId).IsEqualTo(upstream.Id);
        await Assert.That(loaded.ToolName).IsEqualTo("search");
        await Assert.That(loaded.Enabled).IsFalse();
        await Assert.That(loaded.OverrideName).IsEqualTo("web-search");
        await Assert.That(loaded.OverrideDescription).IsEqualTo("Search the web");
    }

    [Test]
    public async Task DuplicateToolNameForSameUpstream_ViolatesUniqueIndex()
    {
        var upstream = new Upstream { Id = Guid.NewGuid(), Name = "upstream", Transport = TransportKind.Stdio };
        Context.Upstreams.Add(upstream);
        await Context.SaveChangesAsync();

        Context.ToolOverrides.Add(new ToolOverride { Id = Guid.NewGuid(), UpstreamId = upstream.Id, ToolName = "search" });
        await Context.SaveChangesAsync();

        Context.ToolOverrides.Add(new ToolOverride { Id = Guid.NewGuid(), UpstreamId = upstream.Id, ToolName = "search" });

        await Assert.That(async () => await Context.SaveChangesAsync()).Throws<DbUpdateException>();
    }

    [Test]
    public async Task DeletingUpstream_CascadesToToolOverrides()
    {
        var upstream = new Upstream { Id = Guid.NewGuid(), Name = "upstream", Transport = TransportKind.Stdio };
        Context.Upstreams.Add(upstream);

        var toolOverride = new ToolOverride { Id = Guid.NewGuid(), UpstreamId = upstream.Id, ToolName = "search" };
        Context.ToolOverrides.Add(toolOverride);
        await Context.SaveChangesAsync();

        Context.Upstreams.Remove(upstream);
        await Context.SaveChangesAsync();

        await using var freshContext = CreateContext();
        var remaining = await freshContext.ToolOverrides.CountAsync(t => t.Id == toolOverride.Id);

        await Assert.That(remaining).IsEqualTo(0);
    }
}
