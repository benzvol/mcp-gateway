using System.Text.Json;

using McpGateway.Core.Domain;
using McpGateway.Core.Domain.Enums;

namespace McpGateway.Core.Upstreams;

internal static class UpstreamAuth
{
    private const string DefaultHeader = "Authorization";
    private const string DefaultScheme = "Bearer";

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public static Dictionary<string, string> BuildHeaders(Upstream upstream)
    {
        var headers = new Dictionary<string, string>(upstream.Headers);

        switch (upstream.AuthKind)
        {
            case AuthKind.None:
                break;
            case AuthKind.ApiKey:
                ApplyApiKey(headers, upstream);
                break;
            case AuthKind.OAuth2:
                ApplyOAuth2(headers, upstream);
                break;
            case AuthKind.CustomHeaders:
                ApplyCustomHeaders(headers, upstream);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(upstream), upstream.AuthKind, "Unsupported auth kind.");
        }

        return headers;
    }

    private static void ApplyApiKey(Dictionary<string, string> headers, Upstream upstream)
    {
        if (string.IsNullOrEmpty(upstream.Secret))
        {
            return;
        }

        var config = ParseApiKeyConfig(upstream.AuthConfigJson);
        var header = config?.Header ?? DefaultHeader;
        var scheme = config?.Scheme ?? (config?.Header is null ? DefaultScheme : null);

        headers[header] = scheme is null ? upstream.Secret : $"{scheme} {upstream.Secret}";
    }

    private static void ApplyOAuth2(Dictionary<string, string> headers, Upstream upstream)
    {
        if (string.IsNullOrEmpty(upstream.Secret))
        {
            return;
        }

        headers[DefaultHeader] = $"{DefaultScheme} {upstream.Secret}";
    }

    private static void ApplyCustomHeaders(Dictionary<string, string> headers, Upstream upstream)
    {
        var custom = ParseCustomHeaders(upstream.AuthConfigJson);
        foreach (var (name, value) in custom)
        {
            headers[name] = value;
        }
    }

    private static ApiKeyConfig? ParseApiKeyConfig(string? authConfigJson)
    {
        if (string.IsNullOrWhiteSpace(authConfigJson))
        {
            return null;
        }

        try
        {
            return JsonSerializer.Deserialize<ApiKeyConfig>(authConfigJson, JsonOptions);
        }
        catch (JsonException)
        {
            return null;
        }
    }

    private static Dictionary<string, string> ParseCustomHeaders(string? authConfigJson)
    {
        if (string.IsNullOrWhiteSpace(authConfigJson))
        {
            return [];
        }

        try
        {
            return JsonSerializer.Deserialize<Dictionary<string, string>>(authConfigJson) ?? [];
        }
        catch (JsonException)
        {
            return [];
        }
    }

    private sealed record ApiKeyConfig(string? Header, string? Scheme);
}
