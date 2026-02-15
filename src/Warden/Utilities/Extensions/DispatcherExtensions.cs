using Avalonia.Threading;

namespace Warden.Utilities.Extensions;

public static class DispatcherExtensions
{
    extension(IDispatcher dispatcher)
    {
        public Task PostAsync(Func<Task> action, DispatcherPriority dispatcherPriority = default)
        {
            var tcs = new TaskCompletionSource();
            dispatcher.Post(
                () =>
                {
                    action().GetAwaiter().GetResult();
                    tcs.SetResult();
                },
                dispatcherPriority
            );
            return tcs.Task;
        }

        public Task<T> PostAsync<T>(
            Func<Task<T>> func,
            DispatcherPriority dispatcherPriority = default
        )
        {
            var tcs = new TaskCompletionSource<T>();
            dispatcher.Post(
                () =>
                {
                    var value = func().GetAwaiter().GetResult();
                    tcs.SetResult(value);
                },
                dispatcherPriority
            );
            return tcs.Task;
        }
    }

    extension(Task task)
    {
        public void WaitOnDispatcherFrame(Dispatcher? dispatcher = null)
        {
            var frame = new DispatcherFrame();
            AggregateException? capturedException = null;

            task.ContinueWith(
                t =>
                {
                    capturedException = t.Exception;
                    frame.Continue = false; // 结束消息循环
                },
                TaskContinuationOptions.AttachedToParent
            );

            dispatcher ??= Dispatcher.UIThread;
            dispatcher.PushFrame(frame);

            if (capturedException != null)
            {
                throw capturedException;
            }
        }
    }
}
