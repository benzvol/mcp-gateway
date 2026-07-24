namespace McpGateway.Core.Domain;

public enum AuthKind
{
    None,
    ApiKey,
    OAuth2,
    CustomHeaders,
}