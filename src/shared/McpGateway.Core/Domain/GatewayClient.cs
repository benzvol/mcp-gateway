using McpGateway.Core.Domain.Enums;

namespace McpGateway.Core.Domain;

public class GatewayClient
{
    public Guid Id { get; init; } = Guid.CreateVersion7();

    public required string Name { get; set; }

    public AuthKind AuthKind { get; set; }

    public string? Secret { get; set; }
}
