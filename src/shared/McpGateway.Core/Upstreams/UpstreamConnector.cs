using McpGateway.Core.Domain;
using McpGateway.Core.Domain.Enums;

using ModelContextProtocol.Client;

namespace McpGateway.Core.Upstreams;

internal sealed class UpstreamConnector : IUpstreamConnector
{
    public Task<McpClient> ConnectAsync(Upstream upstream, CancellationToken cancellationToken = default)
    {
        IClientTransport transport = upstream.Transport switch
        {
            TransportKind.Stdio => BuildStdioTransport(upstream),
            TransportKind.StreamableHttp => BuildHttpTransport(upstream),
            _ => throw new ArgumentOutOfRangeException(nameof(upstream), upstream.Transport,
                "Unsupported transport kind."),
        };

        return McpClient.CreateAsync(transport, cancellationToken: cancellationToken);
    }

    private static StdioClientTransport BuildStdioTransport(Upstream upstream)
    {
        if (UpstreamValidation.StdioRequiresCommand(upstream.Transport, upstream.Command))
        {
            throw new ArgumentException("A stdio upstream requires a command.", nameof(upstream));
        }

        return new StdioClientTransport(new StdioClientTransportOptions
        {
            Name = upstream.Name,
            Command = upstream.Command!,
            Arguments = upstream.Args,
            EnvironmentVariables = upstream.Environment.ToDictionary(
                kvp => kvp.Key,
                kvp => (string?)kvp.Value),
        });
    }

    private static HttpClientTransport BuildHttpTransport(Upstream upstream)
    {
        if (UpstreamValidation.HttpRequiresEndpoint(upstream.Transport, upstream.Endpoint) ||
            !Uri.TryCreate(upstream.Endpoint, UriKind.Absolute, out var endpoint))
        {
            throw new ArgumentException("A Streamable HTTP upstream requires an absolute endpoint URL.",
                nameof(upstream));
        }

        return new HttpClientTransport(new HttpClientTransportOptions
        {
            Name = upstream.Name,
            Endpoint = endpoint,
            TransportMode = HttpTransportMode.StreamableHttp,
            AdditionalHeaders = UpstreamAuth.BuildHeaders(upstream),
        });
    }
}
