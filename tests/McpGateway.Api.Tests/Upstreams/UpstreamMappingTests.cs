using McpGateway.Api.Upstreams;
using McpGateway.Core.Domain;
using McpGateway.Core.Domain.Enums;

namespace McpGateway.Api.Tests.Upstreams;

public class UpstreamMappingTests
{
    [Test]
    public async Task Apply_NullSecretAndAuthConfig_PreservesStoredValues()
    {
        var upstream = NewUpstream(secret: "sk-live-123", authConfigJson: """{"header":"X-Api-Key"}""");

        UpstreamMapping.Apply(RequestWithSecretAndAuthConfig(secret: null, authConfigJson: null), upstream);

        await Assert.That(upstream.Secret).IsEqualTo("sk-live-123");
        await Assert.That(upstream.AuthConfigJson).IsEqualTo("""{"header":"X-Api-Key"}""");
    }

    [Test]
    public async Task Apply_EmptySecret_ClearsStoredSecret()
    {
        var upstream = NewUpstream(secret: "sk-live-123");

        UpstreamMapping.Apply(RequestWithSecretAndAuthConfig(secret: "", authConfigJson: null), upstream);

        await Assert.That(upstream.Secret).IsNull();
    }

    [Test]
    public async Task Apply_NonEmptySecret_ReplacesStoredSecret()
    {
        var upstream = NewUpstream(secret: "sk-live-old");

        UpstreamMapping.Apply(RequestWithSecretAndAuthConfig(secret: "sk-live-new", authConfigJson: null), upstream);

        await Assert.That(upstream.Secret).IsEqualTo("sk-live-new");
    }

    [Test]
    public async Task Apply_EmptyAuthConfigJson_ClearsStoredAuthConfig()
    {
        var upstream = NewUpstream(authConfigJson: """{"header":"X-Api-Key"}""");

        UpstreamMapping.Apply(RequestWithSecretAndAuthConfig(secret: null, authConfigJson: ""), upstream);

        await Assert.That(upstream.AuthConfigJson).IsNull();
    }

    private static Upstream NewUpstream(string? secret = null, string? authConfigJson = null) => new()
    {
        Name = "test-upstream",
        Transport = TransportKind.StreamableHttp,
        Endpoint = "https://upstream.example.com/mcp",
        AuthKind = AuthKind.ApiKey,
        Secret = secret,
        AuthConfigJson = authConfigJson,
    };

    private static UpdateUpstreamRequest RequestWithSecretAndAuthConfig(string? secret, string? authConfigJson) => new(
        "test-upstream",
        TransportKind.StreamableHttp,
        null,
        null,
        null,
        "https://upstream.example.com/mcp",
        null,
        AuthKind.ApiKey,
        authConfigJson,
        secret,
        true);
}
