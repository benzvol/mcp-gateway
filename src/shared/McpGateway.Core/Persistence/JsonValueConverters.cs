using System.Text.Json;

using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace McpGateway.Core.Persistence;

internal static class JsonValueConverters
{
    public static ValueConverter<List<string>, string> StringList { get; } = new(
        v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
        v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new List<string>());

    public static ValueComparer<List<string>> StringListComparer { get; } = new(
        (a, b) => (a ?? new List<string>()).SequenceEqual(b ?? new List<string>()),
        v => v.Aggregate(0, HashCode.Combine),
        v => v.ToList());

    public static ValueConverter<Dictionary<string, string>, string> StringDictionary { get; } = new(
        v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
        v => JsonSerializer.Deserialize<Dictionary<string, string>>(v, (JsonSerializerOptions?)null) ??
             new Dictionary<string, string>());

    public static ValueComparer<Dictionary<string, string>> StringDictionaryComparer { get; } = new(
        (a, b) => DictionariesEqual(a ?? new Dictionary<string, string>(), b ?? new Dictionary<string, string>()),
        v => v.Aggregate(0, (hash, kv) => HashCode.Combine(hash, kv.Key, kv.Value)),
        v => v.ToDictionary(kv => kv.Key, kv => kv.Value));

    private static bool DictionariesEqual(Dictionary<string, string> a, Dictionary<string, string> b)
    {
        if (a.Count != b.Count)
        {
            return false;
        }

        foreach (var (key, value) in a)
        {
            if (!b.TryGetValue(key, out var otherValue) || value != otherValue)
            {
                return false;
            }
        }

        return true;
    }
}