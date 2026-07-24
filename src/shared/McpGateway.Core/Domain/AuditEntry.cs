namespace McpGateway.Core.Domain;

public class AuditEntry
{
    public long Id { get; set; }

    public DateTimeOffset Timestamp { get; set; }

    public string? UpstreamName { get; set; }

    public required string ToolName { get; set; }

    public string? ClientName { get; set; }

    public AuditStatus Status { get; set; }

    public long LatencyMs { get; set; }

    public string? Input { get; set; }

    public string? Output { get; set; }
}