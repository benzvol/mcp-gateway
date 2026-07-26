using System.ComponentModel;
using System.Net;

using ModelContextProtocol;
using ModelContextProtocol.Client;

namespace McpGateway.Core.Upstreams;

internal static class ConnectionFailureCategorizer
{
    public static ConnectionFailureKind Categorize(
        Exception exception, CancellationToken timeoutToken = default
    ) => exception switch
    {
        ArgumentException or Win32Exception => ConnectionFailureKind.Configuration,
        OperationCanceledException => timeoutToken.IsCancellationRequested
            ? ConnectionFailureKind.Timeout
            : ConnectionFailureKind.Unknown,
        HttpRequestException http => CategorizeHttp(http),
        ClientTransportClosedException => ConnectionFailureKind.Unreachable,
        McpException => ConnectionFailureKind.Protocol,
        _ => ConnectionFailureKind.Unknown,
    };

    private static ConnectionFailureKind CategorizeHttp(HttpRequestException exception) => exception.StatusCode switch
    {
        HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden => ConnectionFailureKind.AuthFailed,
        _ => ConnectionFailureKind.Unreachable,
    };
}
