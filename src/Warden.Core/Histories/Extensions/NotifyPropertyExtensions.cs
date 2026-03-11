using System.Collections.Concurrent;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using R3;

namespace Warden.Core.Histories.Extensions;

public static class NotifyPropertyExtensions
{
    private static readonly ConditionalWeakTable<object, Dictionary<string, object?>> OldValues =
        new();

    private static readonly ConcurrentDictionary<(Type, string), PropertyInfo?> PropertyCache =
        new();

    public static IDisposable AsUndo<T>(this T obj, IUndoManager undoManager)
        where T : class, INotifyPropertyChanged, INotifyPropertyChanging
    {
        var oldValues = OldValues.GetOrCreateValue(obj);
        long lastVersion = undoManager.Version;

        obj.PropertyChanging += OnPropertyChanging;
        // obj.PropertyChanged += OnPropertyChanged;

        return Disposable.Create(() =>
        {
            obj.PropertyChanging -= OnPropertyChanging;
            // obj.PropertyChanged -= OnPropertyChanged;
        });

        void OnPropertyChanging(object? sender, PropertyChangingEventArgs args)
        {
            if (string.IsNullOrWhiteSpace(args.PropertyName))
                return;

            var property = GetProperty(obj.GetType(), args.PropertyName);
            if (property == null || !property.CanRead)
                return;

            var oldValue = property.GetValue(obj);
            undoManager.DoOnUndo(() => property.SetValue(obj, oldValue));

            // Only store old value if not already stored
            // if (!oldValues.ContainsKey(args.PropertyName))
            //     oldValues[args.PropertyName] = property.GetValue(obj);
        }

        // void OnPropertyChanged(object? sender, PropertyChangedEventArgs args)
        // {
        //     if (string.IsNullOrWhiteSpace(args.PropertyName))
        //         return;
        //     if (!oldValues.TryGetValue(args.PropertyName, out var oldValue))
        //         return;
        //
        //     // If version changed, this PropertyChanged comes from undo/redo — skip it
        //     if (undoManager.Version != lastVersion)
        //     {
        //         oldValues.Remove(args.PropertyName);
        //         lastVersion = undoManager.Version; // sync version
        //         return;
        //     }
        //
        //     var property = GetProperty(obj.GetType(), args.PropertyName);
        //     if (property == null || !property.CanRead || !property.CanWrite)
        //         return;
        //
        //     var newValue = property.GetValue(obj);
        //     if (!Equals(oldValue, newValue))
        //     {
        //         undoManager.Do(
        //             () =>
        //             {
        //                 isUndoRedo = true;
        //                 property.SetValue(obj, newValue);
        //                 isUndoRedo = false;
        //             },
        //             () =>
        //             {
        //                 isUndoRedo = true;
        //                 property.SetValue(obj, oldValue);
        //                 isUndoRedo = false;
        //             },
        //             $"Redo/Undo for {obj.GetType().Name}-{args.PropertyName}"
        //         );
        //     }
        //
        //     oldValues.Remove(args.PropertyName);
        // }
    }

    private static PropertyInfo? GetProperty(Type type, string name) =>
        PropertyCache.GetOrAdd((type, name), key => key.Item1.GetProperty(key.Item2));
}
