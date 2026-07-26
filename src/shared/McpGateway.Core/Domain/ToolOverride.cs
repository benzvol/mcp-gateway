namespace McpGateway.Core.Domain;

public class ToolOverride
{
    public Guid Id { get; init; } = Guid.CreateVersion7();

    public Guid UpstreamId { get; init; }

    public required string ToolName { get; set; }

    public bool Enabled { get; set; } = true;

    public string? OverrideName { get; set; }

    public string? OverrideDescription { get; set; }
}
