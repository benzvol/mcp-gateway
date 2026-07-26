using McpGateway.Core.Domain.Enums;

namespace McpGateway.Core.Upstreams;

public static class UpstreamValidation
{
    public static bool StdioRequiresCommand(TransportKind transport, string? command) =>
        transport == TransportKind.Stdio && string.IsNullOrWhiteSpace(command);

    public static bool HttpRequiresEndpoint(TransportKind transport, string? endpoint) =>
        transport == TransportKind.StreamableHttp && string.IsNullOrWhiteSpace(endpoint);
}
