using McpGateway.Core.Persistence;
using Microsoft.EntityFrameworkCore;

namespace McpGateway.TestUtils.Persistence;

public abstract class PersistenceTestBase
{
    private string _databasePath = null!;

    protected GatewayDbContext Context { get; private set; } = null!;

    [Before(Test)]
    public async Task SetUpAsync()
    {
        _databasePath = Path.Combine(Path.GetTempPath(), $"gateway-test-{Guid.NewGuid():N}.db");
        Context = CreateContext();
        await Context.Database.MigrateAsync();
    }

    [After(Test)]
    public async Task TearDownAsync()
    {
        await Context.DisposeAsync();
        if (File.Exists(_databasePath))
        {
            File.Delete(_databasePath);
        }
    }

    protected GatewayDbContext CreateContext()
    {
        var optionsBuilder = new DbContextOptionsBuilder<GatewayDbContext>();
        optionsBuilder.UseSqlite($"Data Source={_databasePath};Pooling=false").UseSnakeCaseNamingConvention();
        return new GatewayDbContext(optionsBuilder.Options);
    }
}
