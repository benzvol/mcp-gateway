namespace McpGateway.Core.Domain;

public class Upstream
{
    public Guid Id { get; set; }

    public required string Name { get; set; }

    public bool Enabled { get; set; }

    public TransportKind Transport { get; set; }

    public string? Command { get; set; }

    public List<string> Args { get; set; } = [];

    public Dictionary<string, string> Environment { get; set; } = [];

    public string? Endpoint { get; set; }

    public Dictionary<string, string> Headers { get; set; } = [];

    public AuthKind AuthKind { get; set; }

    public string? AuthConfigJson { get; set; }

    public string? Secret { get; set; }
}