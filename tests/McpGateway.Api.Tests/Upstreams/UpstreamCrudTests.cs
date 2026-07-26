using System.Net;
using System.Net.Http.Json;

using McpGateway.Api.Upstreams;
using McpGateway.Core.Domain.Enums;

namespace McpGateway.Api.Tests.Upstreams;

public class UpstreamCrudTests : UpstreamApiTestBase
{
    [Test]
    public async Task Post_CreatesUpstream_Returns201WithLocation()
    {
        var request = StdioRequest("filesystem");

        var response = await Client.PostAsJsonAsync("/api/upstreams", request);

        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.Created);
        await Assert.That(response.Headers.Location).IsNotNull();

        var body = await response.Content.ReadFromJsonAsync<UpstreamResponse>();
        await Assert.That(body!.Name).IsEqualTo("filesystem");
        await Assert.That(body.Enabled).IsTrue();
    }

    [Test]
    public async Task Post_DuplicateName_Returns409()
    {
        await Client.PostAsJsonAsync("/api/upstreams", StdioRequest("filesystem"));

        var response = await Client.PostAsJsonAsync("/api/upstreams", StdioRequest("filesystem"));

        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.Conflict);
    }

    [Test]
    public async Task Post_StdioWithoutCommand_Returns400()
    {
        var request = new CreateUpstreamRequest(
            "no-command",
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

        var response = await Client.PostAsJsonAsync("/api/upstreams", request);

        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task Post_HttpWithoutEndpoint_Returns400()
    {
        var request = new CreateUpstreamRequest(
            "no-endpoint",
            TransportKind.StreamableHttp,
            null,
            null,
            null,
            null,
            null,
            AuthKind.None,
            null,
            null,
            null);

        var response = await Client.PostAsJsonAsync("/api/upstreams", request);

        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task Get_List_ReturnsSeededUpstreams()
    {
        await Client.PostAsJsonAsync("/api/upstreams", StdioRequest("filesystem"));
        await Client.PostAsJsonAsync("/api/upstreams", HttpRequest("octopus-deploy"));

        var upstreams = await Client.GetFromJsonAsync<UpstreamResponse[]>("/api/upstreams");

        await Assert.That(upstreams!.Length).IsEqualTo(2);
    }

    [Test]
    public async Task Get_ById_NotFound_Returns404()
    {
        var response = await Client.GetAsync($"/api/upstreams/{Guid.NewGuid()}");

        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.NotFound);
    }

    [Test]
    public async Task Put_UpdatesFields()
    {
        var created = await Client.PostAsJsonAsync("/api/upstreams", StdioRequest("filesystem"));
        var upstream = await created.Content.ReadFromJsonAsync<UpstreamResponse>();

        var update = new UpdateUpstreamRequest(
            "filesystem-renamed",
            TransportKind.Stdio,
            "npx",
            ["-y", "@modelcontextprotocol/server-filesystem"],
            [],
            null,
            [],
            AuthKind.None,
            null,
            null,
            false);

        var response = await Client.PutAsJsonAsync($"/api/upstreams/{upstream!.Id}", update);

        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<UpstreamResponse>();
        await Assert.That(body!.Name).IsEqualTo("filesystem-renamed");
        await Assert.That(body.Enabled).IsFalse();
    }

    [Test]
    public async Task Delete_RemovesRow_Returns204()
    {
        var created = await Client.PostAsJsonAsync("/api/upstreams", StdioRequest("filesystem"));
        var upstream = await created.Content.ReadFromJsonAsync<UpstreamResponse>();

        var response = await Client.DeleteAsync($"/api/upstreams/{upstream!.Id}");

        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.NoContent);
        var getResponse = await Client.GetAsync($"/api/upstreams/{upstream.Id}");
        await Assert.That(getResponse.StatusCode).IsEqualTo(HttpStatusCode.NotFound);
    }

    [Test]
    public async Task Post_Enabled_TogglesEnabled()
    {
        var created = await Client.PostAsJsonAsync("/api/upstreams", StdioRequest("filesystem"));
        var upstream = await created.Content.ReadFromJsonAsync<UpstreamResponse>();

        var response = await Client.PostAsJsonAsync(
            $"/api/upstreams/{upstream!.Id}/enabled",
            new SetEnabledRequest(false));

        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.NoContent);
        var reloaded = await Client.GetFromJsonAsync<UpstreamResponse>($"/api/upstreams/{upstream.Id}");
        await Assert.That(reloaded!.Enabled).IsFalse();
    }

    private static CreateUpstreamRequest StdioRequest(string name) => new(
        name,
        TransportKind.Stdio,
        "npx",
        ["-y", "@modelcontextprotocol/server-everything"],
        [],
        null,
        [],
        AuthKind.None,
        null,
        null,
        null);

    private static CreateUpstreamRequest HttpRequest(string name) => new(
        name,
        TransportKind.StreamableHttp,
        null,
        null,
        null,
        "https://upstream.example.com/mcp",
        [],
        AuthKind.None,
        null,
        null,
        null);
}
