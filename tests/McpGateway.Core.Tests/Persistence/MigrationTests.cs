using McpGateway.TestUtils.Persistence;

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace McpGateway.Core.Tests.Persistence;

public class MigrationTests : PersistenceTestBase
{
    [Test]
    public async Task Migration_CreatesExpectedTables()
    {
        var tables = await QueryScalarListAsync(
            "SELECT name FROM sqlite_master WHERE type='table' AND name NOT LIKE 'sqlite_%' AND name NOT LIKE '\\_\\_EFMigrations%' ESCAPE '\\'");

        await Assert.That(tables).IsEquivalentTo(
            ["upstreams", "tool_overrides", "gateway_clients", "audit_entries"]);
    }

    [Test]
    public async Task JournalModePragma_SwitchesToWal()
    {
        var connection = (SqliteConnection)Context.Database.GetDbConnection();
        await connection.OpenAsync();

        await Context.Database.ExecuteSqlRawAsync("PRAGMA journal_mode=WAL;");

        await using var command = connection.CreateCommand();
        command.CommandText = "PRAGMA journal_mode;";
        var mode = (string)(await command.ExecuteScalarAsync())!;

        await Assert.That(mode).IsEqualTo("wal", StringComparison.OrdinalIgnoreCase);
    }

    private async Task<List<string>> QueryScalarListAsync(string sql)
    {
        var connection = (SqliteConnection)Context.Database.GetDbConnection();
        if (connection.State != System.Data.ConnectionState.Open)
        {
            await connection.OpenAsync();
        }

        await using var command = connection.CreateCommand();
        command.CommandText = sql;
        await using var reader = await command.ExecuteReaderAsync();

        var results = new List<string>();
        while (await reader.ReadAsync())
        {
            results.Add(reader.GetString(0));
        }

        return results;
    }
}