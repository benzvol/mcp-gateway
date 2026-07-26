using McpGateway.Core.Domain;
using McpGateway.Core.Upstreams;

namespace McpGateway.Api.Upstreams;

internal static class UpstreamMapping
{
    public static Upstream ToEntity(CreateUpstreamRequest request) => new()
    {
        Name = request.Name,
        Enabled = request.Enabled ?? true,
        Transport = request.Transport,
        Command = request.Command,
        Args = request.Args ?? [],
        Environment = request.Environment ?? [],
        Endpoint = request.Endpoint,
        Headers = request.Headers ?? [],
        AuthKind = request.AuthKind,
        AuthConfigJson = request.AuthConfigJson,
        Secret = request.Secret,
    };

    public static void Apply(UpdateUpstreamRequest request, Upstream upstream)
    {
        upstream.Name = request.Name;
        upstream.Enabled = request.Enabled;
        upstream.Transport = request.Transport;
        upstream.Command = request.Command;
        upstream.Args = request.Args ?? [];
        upstream.Environment = request.Environment ?? [];
        upstream.Endpoint = request.Endpoint;
        upstream.Headers = request.Headers ?? [];
        upstream.AuthKind = request.AuthKind;
        upstream.AuthConfigJson = request.AuthConfigJson;
        upstream.Secret = request.Secret;
    }

    public static UpstreamResponse ToResponse(Upstream upstream) => new(
        upstream.Id,
        upstream.Name,
        upstream.Enabled,
        upstream.Transport,
        upstream.Command,
        upstream.Args,
        upstream.Environment,
        upstream.Endpoint,
        upstream.Headers,
        upstream.AuthKind,
        HasAuthConfig: !string.IsNullOrEmpty(upstream.AuthConfigJson),
        HasSecret: !string.IsNullOrEmpty(upstream.Secret));

    public static TestConnectionResponse ToResponse(ConnectionTestResult result) => new(
        result.Success,
        result.FailureKind.ToString(),
        result.Message,
        result.Latency.TotalMilliseconds,
        result.Tools.Select(t => new ToolPreview(t.Name, t.Description)).ToArray());
}
