using System.Net;
using System.Net.Http.Json;
using System.Net.Sockets;

using McpGateway.Api.Upstreams;
using McpGateway.Core.Domain.Enums;
using McpGateway.Core.Upstreams;

namespace McpGateway.Api.Tests.Upstreams;

public class TestConnectionApiTests : UpstreamApiTestBase
{
    [Test]
    public async Task PostTest_UnreachableEndpoint_ReturnsOkWithUnreachableKind()
    {
        var request = new CreateUpstreamRequest(
            "unreachable",
            TransportKind.StreamableHttp,
            null,
            null,
            null,
            $"http://127.0.0.1:{FindClosedPort()}/mcp",
            [],
            AuthKind.None,
            null,
            null,
            null);

        var response = await Client.PostAsJsonAsync("/api/upstreams/test", request);

        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<TestConnectionResponse>();
        await Assert.That(body!.Success).IsFalse();
        await Assert.That(body.FailureKind).IsEqualTo(nameof(ConnectionFailureKind.Unreachable));
    }

    [Test]
    public async Task PostTest_InvalidPayload_ReturnsBadRequest()
    {
        var request = new CreateUpstreamRequest(
            "missing-command",
            TransportKind.Stdio,
            null,
            null,
            null,
            null,
            null,
            AuthKind.None,
            null,
            null,
            null);

        var response = await Client.PostAsJsonAsync("/api/upstreams/test", request);

        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task PostTestById_SavedUpstream_UsesStoredSecret()
    {
        var createRequest = new CreateUpstreamRequest(
            "saved-unreachable",
            TransportKind.StreamableHttp,
            null,
            null,
            null,
            $"http://127.0.0.1:{FindClosedPort()}/mcp",
            [],
            AuthKind.ApiKey,
            null,
            "sk-live-test",
            null);
        var created = await Client.PostAsJsonAsync("/api/upstreams", createRequest);
        var upstream = await created.Content.ReadFromJsonAsync<UpstreamResponse>(JsonOptions);

        var response = await Client.PostAsync($"/api/upstreams/{upstream!.Id}/test", null);

        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<TestConnectionResponse>();
        await Assert.That(body!.Success).IsFalse();
        await Assert.That(body.FailureKind).IsEqualTo(nameof(ConnectionFailureKind.Unreachable));
    }

    [Test]
    public async Task PostTestById_UnknownId_ReturnsNotFound()
    {
        var response = await Client.PostAsync($"/api/upstreams/{Guid.NewGuid()}/test", null);

        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.NotFound);
    }

    private static int FindClosedPort()
    {
        using var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        var port = ((IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();
        return port;
    }
}
