using McpGateway.Api.Upstreams;
using McpGateway.Core.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services
    .AddGatewayPersistence(builder.Configuration.GetConnectionString("Gateway")!)
    .AddGatewayDatabaseInitializer()
    .AddUpstreamConnectivity();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapUpstreamEndpoints();

app.Run();
