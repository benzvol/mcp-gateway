using System.Net.Http.Json;

using McpGateway.Api.Upstreams;
using McpGateway.Core.Domain.Enums;

namespace McpGateway.Api.Tests.Upstreams;

public class UpstreamRedactionTests : UpstreamApiTestBase
{
    private const string Secret = "sk-live-super-secret-token";

    [Test]
    public async Task GetById_ResponseDoesNotContainSecret()
    {
        var created = await Client.PostAsJsonAsync("/api/upstreams", RequestWithSecret());
        var upstream = await created.Content.ReadFromJsonAsync<UpstreamResponse>(JsonOptions);

        var response = await Client.GetAsync($"/api/upstreams/{upstream!.Id}");
        var raw = await response.Content.ReadAsStringAsync();

        await Assert.That(raw.Contains(Secret)).IsFalse();

        var body = await response.Content.ReadFromJsonAsync<UpstreamResponse>(JsonOptions);
        await Assert.That(body!.HasSecret).IsTrue();
    }

    [Test]
    public async Task List_ResponseDoesNotLeakSecret()
    {
        await Client.PostAsJsonAsync("/api/upstreams", RequestWithSecret());

        var response = await Client.GetAsync("/api/upstreams");
        var raw = await response.Content.ReadAsStringAsync();

        await Assert.That(raw.Contains(Secret)).IsFalse();
    }

    private static CreateUpstreamRequest RequestWithSecret() => new(
        "octopus-deploy",
        TransportKind.StreamableHttp,
        null,
        null,
        null,
        "https://octopus.example.com/mcp",
        [],
        AuthKind.ApiKey,
        null,
        Secret,
        null);
}
