namespace McpGateway.Core.Domain;

public class GatewayClient
{
    public Guid Id { get; set; }

    public required string Name { get; set; }

    public AuthKind AuthKind { get; set; }

    public string? Secret { get; set; }
}