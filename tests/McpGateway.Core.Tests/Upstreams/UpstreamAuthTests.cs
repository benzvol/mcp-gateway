using McpGateway.Core.Domain;
using McpGateway.Core.Domain.Enums;
using McpGateway.Core.Upstreams;

namespace McpGateway.Core.Tests.Upstreams;

public class UpstreamAuthTests
{
    [Test]
    public async Task None_LeavesHeadersUntouched()
    {
        var upstream = NewUpstream(AuthKind.None, headers: new Dictionary<string, string> { ["X-Base"] = "value" });

        var headers = UpstreamAuth.BuildHeaders(upstream);

        await Assert.That(headers).IsEquivalentTo(new Dictionary<string, string> { ["X-Base"] = "value" });
    }

    [Test]
    public async Task ApiKey_DefaultsToAuthorizationBearer()
    {
        var upstream = NewUpstream(AuthKind.ApiKey, secret: "sk-live-123");

        var headers = UpstreamAuth.BuildHeaders(upstream);

        await Assert.That(headers["Authorization"]).IsEqualTo("Bearer sk-live-123");
    }

    [Test]
    public async Task ApiKey_HonorsCustomHeaderNameFromAuthConfigJson()
    {
        var upstream = NewUpstream(
            AuthKind.ApiKey,
            secret: "sk-live-123",
            authConfigJson: """{"header":"X-Api-Key"}""");

        var headers = UpstreamAuth.BuildHeaders(upstream);

        await Assert.That(headers["X-Api-Key"]).IsEqualTo("sk-live-123");
        await Assert.That(headers.ContainsKey("Authorization")).IsFalse();
    }

    [Test]
    public async Task ApiKey_HonorsCustomHeaderAndScheme()
    {
        var upstream = NewUpstream(
            AuthKind.ApiKey,
            secret: "sk-live-123",
            authConfigJson: """{"header":"X-Api-Key","scheme":"Token"}""");

        var headers = UpstreamAuth.BuildHeaders(upstream);

        await Assert.That(headers["X-Api-Key"]).IsEqualTo("Token sk-live-123");
    }

    [Test]
    public async Task ApiKey_NullOrBlankAuthConfigJson_FallsBackToDefaults()
    {
        var upstream = NewUpstream(AuthKind.ApiKey, secret: "sk-live-123", authConfigJson: "   ");

        var headers = UpstreamAuth.BuildHeaders(upstream);

        await Assert.That(headers["Authorization"]).IsEqualTo("Bearer sk-live-123");
    }

    [Test]
    public async Task OAuth2_UsesSecretAsBearerToken()
    {
        var upstream = NewUpstream(AuthKind.OAuth2, secret: "access-token-abc");

        var headers = UpstreamAuth.BuildHeaders(upstream);

        await Assert.That(headers["Authorization"]).IsEqualTo("Bearer access-token-abc");
    }

    [Test]
    public async Task CustomHeaders_MergesJsonHeaders()
    {
        var upstream = NewUpstream(
            AuthKind.CustomHeaders,
            authConfigJson: """{"X-Custom":"v","X-Other":"v2"}""");

        var headers = UpstreamAuth.BuildHeaders(upstream);

        await Assert.That(headers["X-Custom"]).IsEqualTo("v");
        await Assert.That(headers["X-Other"]).IsEqualTo("v2");
    }

    [Test]
    public async Task CustomHeaders_AuthHeadersOverrideBaseHeaders()
    {
        var upstream = NewUpstream(
            AuthKind.CustomHeaders,
            headers: new Dictionary<string, string> { ["X-Custom"] = "base-value" },
            authConfigJson: """{"X-Custom":"override-value"}""");

        var headers = UpstreamAuth.BuildHeaders(upstream);

        await Assert.That(headers["X-Custom"]).IsEqualTo("override-value");
    }

    [Test]
    public async Task CustomHeaders_NullAuthConfigJson_LeavesBaseHeadersUntouched()
    {
        var upstream = NewUpstream(
            AuthKind.CustomHeaders,
            headers: new Dictionary<string, string> { ["X-Base"] = "value" });

        var headers = UpstreamAuth.BuildHeaders(upstream);

        await Assert.That(headers).IsEquivalentTo(new Dictionary<string, string> { ["X-Base"] = "value" });
    }

    private static Upstream NewUpstream(
        AuthKind authKind,
        Dictionary<string, string>? headers = null,
        string? secret = null,
        string? authConfigJson = null
    ) => new()
    {
        Name = "test-upstream",
        Transport = TransportKind.StreamableHttp,
        Endpoint = "https://upstream.example.com/mcp",
        Headers = headers ?? [],
        AuthKind = authKind,
        Secret = secret,
        AuthConfigJson = authConfigJson,
    };
}
