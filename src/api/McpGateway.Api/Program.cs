using McpGateway.Api.Upstreams;
using McpGateway.Core.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services
    .AddGatewayPersistence(builder.Configuration.GetConnectionString("Gateway")!)
    .AddGatewayDatabaseInitializer()
    .AddUpstreamConnectivity();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapUpstreamEndpoints();

app.Run();

public partial class Program;
