using System.Text.Json;
using System.Text.Json.Serialization;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace McpGateway.Api.Tests.Upstreams;

public abstract class UpstreamApiTestBase : WebApplicationFactory<Program>
{
    protected static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() },
    };

    private string _databasePath = null!;

    protected HttpClient Client { get; private set; } = null!;

    [Before(Test)]
    public Task SetUpAsync()
    {
        _databasePath = Path.Combine(Path.GetTempPath(), $"gateway-api-test-{Guid.NewGuid():N}.db");
        Client = CreateClient();
        return Task.CompletedTask;
    }

    [After(Test)]
    public Task TearDownAsync()
    {
        Client.Dispose();
        if (File.Exists(_databasePath))
        {
            File.Delete(_databasePath);
        }

        return Task.CompletedTask;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting("ConnectionStrings:Gateway", $"Data Source={_databasePath};Pooling=false");
    }
}
