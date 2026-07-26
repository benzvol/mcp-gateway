using McpGateway.Core.Domain.Enums;
using McpGateway.Core.Persistence;
using McpGateway.Core.Upstreams;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace McpGateway.Api.Upstreams;

public static class UpstreamEndpoints
{
    extension(IEndpointRouteBuilder app)
    {
        public void MapUpstreamEndpoints()
        {
            var group = app.MapGroup("/api/upstreams");

            group.MapGet("/", ListUpstreams);
            group.MapGet("/{id:guid}", GetUpstream);
            group.MapPost("/", CreateUpstream);
            group.MapPut("/{id:guid}", UpdateUpstream);
            group.MapDelete("/{id:guid}", DeleteUpstream);
            group.MapPost("/{id:guid}/enabled", SetEnabled);
            group.MapPost("/test", TestUnsavedConnection);
            group.MapPost("/{id:guid}/test", TestSavedConnection);
        }
    }

    private static async Task<Ok<UpstreamResponse[]>> ListUpstreams(GatewayDbContext db)
    {
        var upstreams = await db.Upstreams.AsNoTracking().ToListAsync();
        return TypedResults.Ok(upstreams.Select(UpstreamMapping.ToResponse).ToArray());
    }

    private static async Task<Results<Ok<UpstreamResponse>, NotFound>> GetUpstream(Guid id, GatewayDbContext db)
    {
        var upstream = await db.Upstreams.AsNoTracking().SingleOrDefaultAsync(u => u.Id == id);
        return upstream is null
            ? TypedResults.NotFound()
            : TypedResults.Ok(UpstreamMapping.ToResponse(upstream));
    }

    private static async Task<Results<Created<UpstreamResponse>, ValidationProblem, Conflict<string>>> CreateUpstream(
        CreateUpstreamRequest request,
        GatewayDbContext db
    )
    {
        var validationErrors = Validate(request.Transport, request.Command, request.Endpoint);
        if (validationErrors is not null)
        {
            return TypedResults.ValidationProblem(validationErrors);
        }

        var upstream = UpstreamMapping.ToEntity(request);
        db.Upstreams.Add(upstream);

        try
        {
            await db.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return TypedResults.Conflict($"An upstream named '{request.Name}' already exists.");
        }

        var response = UpstreamMapping.ToResponse(upstream);
        return TypedResults.Created($"/api/upstreams/{upstream.Id}", response);
    }

    private static async Task<Results<Ok<UpstreamResponse>, NotFound, ValidationProblem, Conflict<string>>>
        UpdateUpstream(
            Guid id,
            UpdateUpstreamRequest request,
            GatewayDbContext db
        )
    {
        var upstream = await db.Upstreams.SingleOrDefaultAsync(u => u.Id == id);
        if (upstream is null)
        {
            return TypedResults.NotFound();
        }

        var validationErrors = Validate(request.Transport, request.Command, request.Endpoint);
        if (validationErrors is not null)
        {
            return TypedResults.ValidationProblem(validationErrors);
        }

        UpstreamMapping.Apply(request, upstream);

        try
        {
            await db.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return TypedResults.Conflict($"An upstream named '{request.Name}' already exists.");
        }

        return TypedResults.Ok(UpstreamMapping.ToResponse(upstream));
    }

    private static async Task<Results<NoContent, NotFound>> DeleteUpstream(Guid id, GatewayDbContext db)
    {
        var upstream = await db.Upstreams.SingleOrDefaultAsync(u => u.Id == id);
        if (upstream is null)
        {
            return TypedResults.NotFound();
        }

        db.Upstreams.Remove(upstream);
        await db.SaveChangesAsync();
        return TypedResults.NoContent();
    }

    private static async Task<Results<NoContent, NotFound>> SetEnabled(
        Guid id, SetEnabledRequest request, GatewayDbContext db
    )
    {
        var upstream = await db.Upstreams.SingleOrDefaultAsync(u => u.Id == id);
        if (upstream is null)
        {
            return TypedResults.NotFound();
        }

        upstream.Enabled = request.Enabled;
        await db.SaveChangesAsync();
        return TypedResults.NoContent();
    }

    private static async Task<Results<Ok<TestConnectionResponse>, ValidationProblem>> TestUnsavedConnection(
        CreateUpstreamRequest request,
        IConnectionTester tester
    )
    {
        var validationErrors = Validate(request.Transport, request.Command, request.Endpoint);
        if (validationErrors is not null)
        {
            return TypedResults.ValidationProblem(validationErrors);
        }

        var upstream = UpstreamMapping.ToEntity(request);
        var result = await tester.TestAsync(upstream);
        return TypedResults.Ok(UpstreamMapping.ToResponse(result));
    }

    private static async Task<Results<Ok<TestConnectionResponse>, NotFound>> TestSavedConnection(
        Guid id,
        GatewayDbContext db,
        IConnectionTester tester
    )
    {
        var upstream = await db.Upstreams.AsNoTracking().SingleOrDefaultAsync(u => u.Id == id);
        if (upstream is null)
        {
            return TypedResults.NotFound();
        }

        var result = await tester.TestAsync(upstream);
        return TypedResults.Ok(UpstreamMapping.ToResponse(result));
    }

    private static Dictionary<string, string[]>? Validate(TransportKind transport, string? command, string? endpoint)
    {
        var errors = new Dictionary<string, string[]>();

        if (transport == TransportKind.Stdio && string.IsNullOrWhiteSpace(command))
        {
            errors[nameof(command)] = ["A stdio upstream requires a command."];
        }

        if (transport == TransportKind.StreamableHttp && string.IsNullOrWhiteSpace(endpoint))
        {
            errors[nameof(endpoint)] = ["A Streamable HTTP upstream requires an endpoint URL."];
        }

        return errors.Count == 0 ? null : errors;
    }
}
