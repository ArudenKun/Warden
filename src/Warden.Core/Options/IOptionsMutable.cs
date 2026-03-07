using Microsoft.Extensions.Options;

namespace Warden.Core.Options;

/// <summary>
/// Used to access or update the value of <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">Options type.</typeparam>
// ReSharper disable once PossibleInterfaceMemberAmbiguity
public interface IOptionsMutable<out T> : IOptionsSnapshot<T>, IOptionsMonitor<T>
    where T : class, new()
{
    /// <summary>
    /// Update options field by field
    /// </summary>
    /// <param name="applyChanges"></param>
    /// <returns></returns>
    ValueTask<bool> UpdateAsync(Action<T> applyChanges);
}
