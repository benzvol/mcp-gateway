using System.Net;

using ModelContextProtocol;
using ModelContextProtocol.Client;

namespace McpGateway.Core.Upstreams;

internal static class ConnectionFailureCategorizer
{
    public static ConnectionFailureKind Categorize(Exception exception) => exception switch
    {
        ArgumentException => ConnectionFailureKind.Configuration,
        OperationCanceledException => ConnectionFailureKind.Timeout,
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
