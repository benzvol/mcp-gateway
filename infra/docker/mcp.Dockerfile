# syntax=docker/dockerfile:1
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY global.json nuget.config Directory.Build.props Directory.Packages.props ./
COPY src/shared/McpGateway.Core/McpGateway.Core.csproj src/shared/McpGateway.Core/
COPY src/mcp/McpGateway.Mcp/McpGateway.Mcp.csproj src/mcp/McpGateway.Mcp/
RUN dotnet restore src/mcp/McpGateway.Mcp/McpGateway.Mcp.csproj

COPY src/shared/McpGateway.Core/ src/shared/McpGateway.Core/
COPY src/mcp/McpGateway.Mcp/ src/mcp/McpGateway.Mcp/
RUN dotnet publish src/mcp/McpGateway.Mcp/McpGateway.Mcp.csproj \
    -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app ./
EXPOSE 8080
ENTRYPOINT ["dotnet", "McpGateway.Mcp.dll"]
