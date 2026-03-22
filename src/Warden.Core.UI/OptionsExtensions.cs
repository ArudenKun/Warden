using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using R3;
using Warden.Core.Options;

namespace Warden.Core;

file static class TypeCache<T>
{
    public static readonly Type Value = typeof(T);
}

file static class PropertyCopyCache<T>
{
    public static readonly Dictionary<string, Action<T, T>> Setters = Build();

    private static Dictionary<string, Action<T, T>> Build()
    {
        var type = TypeCache<T>.Value;
        var result = new Dictionary<string, Action<T, T>>();

        foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            if (!prop.CanRead || !prop.CanWrite)
                continue;

            var target = Expression.Parameter(type, "target");
            var source = Expression.Parameter(type, "source");

            var assign = Expression.Assign(
                Expression.Property(target, prop),
                Expression.Property(source, prop)
            );
            var lambda = Expression.Lambda<Action<T, T>>(assign, target, source).Compile();
            result[prop.Name] = lambda;
        }

        return result;
    }
}

public static class OptionsExtensions
{
    public static IDisposable AutoUpdate<T>(this T obj, IServiceProvider serviceProvider)
        where T : class, INotifyPropertyChanged, INotifyPropertyChanging, new() =>
        obj.PropertyChangedWrapper()
            .Where(e => !string.IsNullOrEmpty(e.PropertyName))
            .Scan(
                new HashSet<string>(),
                (set, args) =>
                {
                    set.Add(args.PropertyName!);
                    return set;
                }
            )
            .Debounce(TimeSpan.FromMilliseconds(350))
            .Select(set =>
            {
                var changed = set.ToArray();
                set.Clear();
                return changed;
            })
            .SubscribeAwait(
                async (changed, _) =>
                {
                    if (changed.IsNullOrEmpty())
                        return;

                    await using var scope = serviceProvider.CreateAsyncScope();
                    var optionsMutable = scope.ServiceProvider.GetRequiredService<
                        IOptionsMutable<T>
                    >();

                    await optionsMutable.UpdateAsync(opt =>
                    {
                        foreach (var name in changed)
                        {
                            if (PropertyCopyCache<T>.Setters.TryGetValue(name, out var setter))
                                setter(opt, obj);
                        }
                    });
                }
            );

    private static Observable<PropertyChangedEventArgs> PropertyChangedWrapper(
        this INotifyPropertyChanged data
    ) =>
        Observable.Create<PropertyChangedEventArgs>(obs =>
        {
            data.PropertyChanged += Handler;
            return Disposable.Create(() => data.PropertyChanged -= Handler);
            void Handler(object? sender, PropertyChangedEventArgs e) => obs.OnNext(e);
        });
}
