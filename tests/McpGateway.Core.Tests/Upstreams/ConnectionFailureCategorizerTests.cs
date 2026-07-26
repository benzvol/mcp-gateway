using System.Net;

using McpGateway.Core.Upstreams;

using ModelContextProtocol;

namespace McpGateway.Core.Tests.Upstreams;

public class ConnectionFailureCategorizerTests
{
    [Test]
    public async Task ArgumentException_MapsToConfiguration()
    {
        var kind = ConnectionFailureCategorizer.Categorize(new ArgumentException("bad config"));

        await Assert.That(kind).IsEqualTo(ConnectionFailureKind.Configuration);
    }

    [Test]
    public async Task OperationCanceledException_MapsToTimeout()
    {
        var kind = ConnectionFailureCategorizer.Categorize(new OperationCanceledException());

        await Assert.That(kind).IsEqualTo(ConnectionFailureKind.Timeout);
    }

    [Test]
    [Arguments(HttpStatusCode.Unauthorized)]
    [Arguments(HttpStatusCode.Forbidden)]
    public async Task HttpRequestException_WithAuthStatusCode_MapsToAuthFailed(HttpStatusCode statusCode)
    {
        var exception = new HttpRequestException("unauthorized", inner: null, statusCode);

        var kind = ConnectionFailureCategorizer.Categorize(exception);

        await Assert.That(kind).IsEqualTo(ConnectionFailureKind.AuthFailed);
    }

    [Test]
    public async Task HttpRequestException_WithoutStatusCode_MapsToUnreachable()
    {
        var exception = new HttpRequestException("connection refused");

        var kind = ConnectionFailureCategorizer.Categorize(exception);

        await Assert.That(kind).IsEqualTo(ConnectionFailureKind.Unreachable);
    }

    [Test]
    public async Task HttpRequestException_WithNonAuthStatusCode_MapsToUnreachable()
    {
        var exception = new HttpRequestException("not found", inner: null, HttpStatusCode.NotFound);

        var kind = ConnectionFailureCategorizer.Categorize(exception);

        await Assert.That(kind).IsEqualTo(ConnectionFailureKind.Unreachable);
    }

    [Test]
    public async Task McpException_MapsToProtocol()
    {
        var kind = ConnectionFailureCategorizer.Categorize(new McpException("protocol error"));

        await Assert.That(kind).IsEqualTo(ConnectionFailureKind.Protocol);
    }

    [Test]
    public async Task UnknownException_MapsToUnknown()
    {
        var kind = ConnectionFailureCategorizer.Categorize(new InvalidOperationException("boom"));

        await Assert.That(kind).IsEqualTo(ConnectionFailureKind.Unknown);
    }
}
