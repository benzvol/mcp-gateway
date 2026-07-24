using McpGateway.Core.Persistence;

namespace McpGateway.Core.Tests.Persistence;

public class JsonValueConvertersTests
{
    [Test]
    public async Task StringDictionaryComparer_SameEntriesDifferentOrder_AreEqualAndHaveSameHash()
    {
        var a = new Dictionary<string, string> { ["A"] = "1", ["B"] = "2" };
        var b = new Dictionary<string, string> { ["B"] = "2", ["A"] = "1" };

        var comparer = JsonValueConverters.StringDictionaryComparer;

        await Assert.That(comparer.Equals(a, b)).IsTrue();
        await Assert.That(comparer.GetHashCode(a)).IsEqualTo(comparer.GetHashCode(b));
    }
}
