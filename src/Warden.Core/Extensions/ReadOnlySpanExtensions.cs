namespace Warden.Core.Extensions;

public static class ReadOnlySpanExtensions
{
    public static bool Any<T>(this ReadOnlySpan<T> source, Predicate<T> predicate)
    {
        foreach (ref readonly var item in source)
        {
            if (predicate(item))
            {
                return true;
            }
        }

        return false;
    }
}
