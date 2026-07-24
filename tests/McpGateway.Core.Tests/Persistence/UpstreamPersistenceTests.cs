using McpGateway.Core.Domain;
using McpGateway.TestUtils.Persistence;

using Microsoft.EntityFrameworkCore;

namespace McpGateway.Core.Tests.Persistence;

public class UpstreamPersistenceTests : PersistenceTestBase
{
    [Test]
    public async Task StdioUpstream_RoundTripsCommandArgsAndEnvironment()
    {
        var upstream = new Upstream
        {
            Id = Guid.NewGuid(),
            Name = "stdio-upstream",
            Enabled = true,
            Transport = TransportKind.Stdio,
            Command = "npx",
            Args = ["-y", "@modelcontextprotocol/server-everything"],
            Environment = new Dictionary<string, string> { ["API_KEY"] = "secret-value" },
            AuthKind = AuthKind.None,
        };

        Context.Upstreams.Add(upstream);
        await Context.SaveChangesAsync();

        await using var freshContext = CreateContext();
        var loaded = await freshContext.Upstreams.SingleAsync(u => u.Id == upstream.Id);

        await Assert.That(loaded.Transport).IsEqualTo(TransportKind.Stdio);
        await Assert.That(loaded.Command).IsEqualTo("npx");
        await Assert.That(loaded.Args).IsEquivalentTo(upstream.Args, TUnit.Assertions.Enums.CollectionOrdering.Matching);
        await Assert.That(loaded.Environment["API_KEY"]).IsEqualTo("secret-value");
    }

    [Test]
    public async Task HttpUpstream_RoundTripsEndpointAndHeaders()
    {
        var upstream = new Upstream
        {
            Id = Guid.NewGuid(),
            Name = "http-upstream",
            Enabled = true,
            Transport = TransportKind.StreamableHttp,
            Endpoint = "https://upstream.example.com/mcp",
            Headers = new Dictionary<string, string> { ["X-Custom"] = "value" },
            AuthKind = AuthKind.CustomHeaders,
        };

        Context.Upstreams.Add(upstream);
        await Context.SaveChangesAsync();

        await using var freshContext = CreateContext();
        var loaded = await freshContext.Upstreams.SingleAsync(u => u.Id == upstream.Id);

        await Assert.That(loaded.Transport).IsEqualTo(TransportKind.StreamableHttp);
        await Assert.That(loaded.Endpoint).IsEqualTo("https://upstream.example.com/mcp");
        await Assert.That(loaded.Headers["X-Custom"]).IsEqualTo("value");
    }

    [Test]
    [Arguments(AuthKind.None)]
    [Arguments(AuthKind.ApiKey)]
    [Arguments(AuthKind.OAuth2)]
    [Arguments(AuthKind.CustomHeaders)]
    public async Task Upstream_RoundTripsEachAuthKindAndSecret(AuthKind authKind)
    {
        var upstream = new Upstream
        {
            Id = Guid.NewGuid(),
            Name = $"upstream-{authKind}",
            Enabled = true,
            Transport = TransportKind.StreamableHttp,
            Endpoint = "https://upstream.example.com/mcp",
            AuthKind = authKind,
            AuthConfigJson = """{"scope":"read"}""",
            Secret = "super-secret-token",
        };

        Context.Upstreams.Add(upstream);
        await Context.SaveChangesAsync();

        await using var freshContext = CreateContext();
        var loaded = await freshContext.Upstreams.SingleAsync(u => u.Id == upstream.Id);

        await Assert.That(loaded.AuthKind).IsEqualTo(authKind);
        await Assert.That(loaded.AuthConfigJson).IsEqualTo(upstream.AuthConfigJson);
        await Assert.That(loaded.Secret).IsEqualTo("super-secret-token");
    }
}
