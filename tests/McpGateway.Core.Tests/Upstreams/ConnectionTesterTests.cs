using System.IO.Pipelines;

using McpGateway.Core.Domain;
using McpGateway.Core.Domain.Enums;
using McpGateway.Core.Upstreams;

using Microsoft.Extensions.Options;

using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;

namespace McpGateway.Core.Tests.Upstreams;

public class ConnectionTesterTests
{
    [Test]
    public async Task Success_ReturnsDiscoveredToolsAndLatency()
    {
        var clientToServer = new Pipe();
        var serverToClient = new Pipe();

        await using var server = McpServer.Create(
            new StreamServerTransport(clientToServer.Reader.AsStream(), serverToClient.Writer.AsStream()),
            new McpServerOptions
            {
                ToolCollection =
                [
                    McpServerTool.Create((string message) => message,
                        new() { Name = "echo", Description = "Echoes input" }),
                    McpServerTool.Create((string message) => message, new() { Name = "shout" }),
                ],
            });
        _ = server.RunAsync();

        var connector = new FakeUpstreamConnector(() => McpClient.CreateAsync(
            new StreamClientTransport(clientToServer.Writer.AsStream(), serverToClient.Reader.AsStream())));
        var tester = new ConnectionTester(connector, TestOptions());

        var result = await tester.TestAsync(NewUpstream());

        await Assert.That(result.Success).IsTrue();
        await Assert.That(result.Tools.Count).IsEqualTo(2);
        await Assert.That(result.Tools.Select(t => t.Name)).Contains("echo");
        await Assert.That(result.Tools.First(t => t.Name == "echo").Description).IsEqualTo("Echoes input");
        await Assert.That(result.Latency).IsGreaterThanOrEqualTo(TimeSpan.Zero);
    }

    [Test]
    public async Task Timeout_WhenConnectHangs_ReturnsTimeoutKind()
    {
        var connector = new FakeUpstreamConnector(async () =>
        {
            await Task.Delay(TimeSpan.FromSeconds(5));
            throw new InvalidOperationException("should have been cancelled before this point");
        });
        var tester = new ConnectionTester(connector, TestOptions(TimeSpan.FromMilliseconds(50)));

        var result = await tester.TestAsync(NewUpstream());

        await Assert.That(result.Success).IsFalse();
        await Assert.That(result.FailureKind).IsEqualTo(ConnectionFailureKind.Timeout);
    }

    [Test]
    public async Task ConfigurationError_FromRealConnector_ReturnsConfigurationKind()
    {
        var tester = new ConnectionTester(new UpstreamConnector(), TestOptions());

        var result = await tester.TestAsync(new Upstream
            { Name = "invalid", Transport = TransportKind.Stdio, Command = null });

        await Assert.That(result.Success).IsFalse();
        await Assert.That(result.FailureKind).IsEqualTo(ConnectionFailureKind.Configuration);
    }

    private static IOptions<ConnectionTestOptions> TestOptions(TimeSpan? timeout = null) =>
        Options.Create(new ConnectionTestOptions { Timeout = timeout ?? TimeSpan.FromSeconds(10) });

    private static Upstream NewUpstream() => new()
    {
        Name = "fake-upstream",
        Transport = TransportKind.Stdio,
        Command = "unused",
    };

    private sealed class FakeUpstreamConnector(Func<Task<McpClient>> connect) : IUpstreamConnector
    {
        public Task<McpClient> ConnectAsync(Upstream upstream, CancellationToken cancellationToken = default) =>
            connect().WaitAsync(cancellationToken);
    }
}
