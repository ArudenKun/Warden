using System.Collections.Frozen;
using System.Collections.Immutable;
using Warden.Core.Modrinth.Extensions;

namespace Warden.Core.Modrinth;

public struct ModrinthQueryStrings
{
    public ModrinthQueryStrings(params IEnumerable<string> ids)
    {
        Value = ids.ToModrinthQueryString();
    }

    public string Value { get; }

    public override string ToString() => Value;

    public static implicit operator ModrinthQueryStrings(ReadOnlySpan<string> ids) =>
        new(ids.ToArray());

    public static implicit operator ModrinthQueryStrings(List<string> ids) => new(ids);

    public static implicit operator ModrinthQueryStrings(HashSet<string> ids) => new(ids);

    public static implicit operator ModrinthQueryStrings(ImmutableArray<string> ids) => new(ids);

    public static implicit operator ModrinthQueryStrings(ImmutableList<string> ids) => new(ids);

    public static implicit operator ModrinthQueryStrings(FrozenSet<string> ids) => new(ids);
}
