# syntax=docker/dockerfile:1
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY global.json nuget.config Directory.Build.props Directory.Packages.props ./
COPY src/shared/McpGateway.Core/McpGateway.Core.csproj src/shared/McpGateway.Core/
COPY src/api/McpGateway.Api/McpGateway.Api.csproj src/api/McpGateway.Api/
RUN dotnet restore src/api/McpGateway.Api/McpGateway.Api.csproj

COPY src/shared/McpGateway.Core/ src/shared/McpGateway.Core/
COPY src/api/McpGateway.Api/ src/api/McpGateway.Api/
RUN dotnet publish src/api/McpGateway.Api/McpGateway.Api.csproj \
    -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app ./
EXPOSE 8080
ENTRYPOINT ["dotnet", "McpGateway.Api.dll"]
