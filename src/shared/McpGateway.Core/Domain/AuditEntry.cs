using McpGateway.Core.Domain.Enums;

namespace McpGateway.Core.Domain;

public class AuditEntry
{
    public long Id { get; init; }

    public DateTimeOffset Timestamp { get; init; }

    public string? UpstreamName { get; set; }

    public required string ToolName { get; set; }

    public string? ClientName { get; set; }

    public ToolUseStatus Status { get; set; }

    public long LatencyMs { get; set; }

    public string? Input { get; set; }

    public string? Output { get; set; }
}
