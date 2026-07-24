using McpGateway.Core.Domain;
using McpGateway.TestUtils.Persistence;

using Microsoft.EntityFrameworkCore;

namespace McpGateway.Core.Tests.Persistence;

public class GatewayClientPersistenceTests : PersistenceTestBase
{
    [Test]
    public async Task GatewayClient_RoundTripsIdNameAndAuth()
    {
        var client = new GatewayClient
        {
            Id = Guid.NewGuid(),
            Name = "claude-code",
            AuthKind = AuthKind.ApiKey,
            Secret = "client-secret",
        };

        Context.GatewayClients.Add(client);
        await Context.SaveChangesAsync();

        await using var freshContext = CreateContext();
        var loaded = await freshContext.GatewayClients.SingleAsync(c => c.Id == client.Id);

        await Assert.That(loaded.Name).IsEqualTo("claude-code");
        await Assert.That(loaded.AuthKind).IsEqualTo(AuthKind.ApiKey);
        await Assert.That(loaded.Secret).IsEqualTo("client-secret");
    }

    [Test]
    public async Task DuplicateClientName_ViolatesUniqueIndex()
    {
        Context.GatewayClients.Add(new GatewayClient { Id = Guid.NewGuid(), Name = "claude-code", AuthKind = AuthKind.None });
        await Context.SaveChangesAsync();

        Context.GatewayClients.Add(new GatewayClient { Id = Guid.NewGuid(), Name = "claude-code", AuthKind = AuthKind.None });

        await Assert.That(async () => await Context.SaveChangesAsync()).Throws<DbUpdateException>();
    }
}
