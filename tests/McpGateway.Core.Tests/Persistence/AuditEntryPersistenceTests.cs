using McpGateway.Core.Domain;
using McpGateway.Core.Domain.Enums;
using McpGateway.TestUtils.Persistence;

using Microsoft.EntityFrameworkCore;

namespace McpGateway.Core.Tests.Persistence;

public class AuditEntryPersistenceTests : PersistenceTestBase
{
    [Test]
    public async Task AuditEntry_DefaultLevel_RoundTripsWithoutInputOutput()
    {
        var entry = new AuditEntry
        {
            Timestamp = DateTimeOffset.UtcNow,
            UpstreamName = "upstream",
            ToolName = "search",
            ClientName = "claude-code",
            Status = ToolUseStatus.Success,
            LatencyMs = 42,
        };

        Context.AuditEntries.Add(entry);
        await Context.SaveChangesAsync();

        await Assert.That(entry.Id).IsGreaterThan(0);

        await using var freshContext = CreateContext();
        var loaded = await freshContext.AuditEntries.SingleAsync(a => a.Id == entry.Id);

        await Assert.That(loaded.Status).IsEqualTo(ToolUseStatus.Success);
        await Assert.That(loaded.LatencyMs).IsEqualTo(42L);
        await Assert.That(loaded.Input).IsNull();
        await Assert.That(loaded.Output).IsNull();
    }

    [Test]
    public async Task AuditEntry_DetailedLevel_RoundTripsInputAndOutput()
    {
        var entry = new AuditEntry
        {
            Timestamp = DateTimeOffset.UtcNow,
            ToolName = "search",
            Status = ToolUseStatus.Error,
            LatencyMs = 7,
            Input = """{"query":"test"}""",
            Output = """{"error":"timeout"}""",
        };

        Context.AuditEntries.Add(entry);
        await Context.SaveChangesAsync();

        await using var freshContext = CreateContext();
        var loaded = await freshContext.AuditEntries.SingleAsync(a => a.Id == entry.Id);

        await Assert.That(loaded.Input).IsEqualTo(entry.Input);
        await Assert.That(loaded.Output).IsEqualTo(entry.Output);
    }

    [Test]
    public async Task Query_FiltersByTimestampClientAndTool_UsesIndexedColumns()
    {
        var now = DateTimeOffset.UtcNow;
        Context.AuditEntries.AddRange(
            new AuditEntry
            {
                Timestamp = now, ToolName = "search", ClientName = "claude-code", Status = ToolUseStatus.Success,
                LatencyMs = 1
            },
            new AuditEntry
            {
                Timestamp = now, ToolName = "fetch", ClientName = "claude-code", Status = ToolUseStatus.Success,
                LatencyMs = 2
            },
            new AuditEntry
            {
                Timestamp = now, ToolName = "search", ClientName = "other-client", Status = ToolUseStatus.Success,
                LatencyMs = 3
            });
        await Context.SaveChangesAsync();

        await using var freshContext = CreateContext();
        var matches = await freshContext.AuditEntries
            .Where(a => a.Timestamp == now && a.ClientName == "claude-code" && a.ToolName == "search")
            .ToListAsync();

        await Assert.That(matches).Count().IsEqualTo(1);
        await Assert.That(matches[0].LatencyMs).IsEqualTo(1L);
    }
}
