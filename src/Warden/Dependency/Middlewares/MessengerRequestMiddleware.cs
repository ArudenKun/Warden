using Autofac.Core.Resolving.Pipeline;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using ServiceScan.SourceGenerator;
using Warden.Messaging;

namespace Warden.Dependency.Middlewares;

public sealed partial class MessengerRequestMiddleware : IResolveMiddleware
{
    public PipelinePhase Phase => PipelinePhase.Activation;

    public void Execute(ResolveRequestContext context, Action<ResolveRequestContext> next)
    {
        next(context);

        if (!context.NewInstanceActivated)
            return;

        var instance = context.Instance!;
        RegisterRequest(instance, null);
    }

    [GenerateServiceRegistrations(
        AssignableTo = typeof(IRequest<,>),
        CustomHandler = nameof(RegisterRequestHandler)
    )]
    public static partial void RegisterRequest(object instance, IMessenger? messenger);

    // [GenerateServiceRegistrations(
    //     AssignableTo = typeof(IAsyncRequest<,>),
    //     CustomHandler = nameof(RegisterRequestHandler)
    // )]
    // public static partial void RegisterAsyncRequest(object instance, IMessenger? messenger);
    //
    // [GenerateServiceRegistrations(
    //     AssignableTo = typeof(IAsyncRequest<,>),
    //     CustomHandler = nameof(RegisterRequestHandler)
    // )]
    // public static partial void RegisterCollectionRequest(object instance, IMessenger? messenger);

    private static void RegisterRequestHandler<TSelf, TSelf2, TMessage>(
        object instance,
        IMessenger? messenger
    )
        where TSelf : class, IRequest<TSelf, TMessage>
    {
        messenger ??= WeakReferenceMessenger.Default;

        if (instance is not TSelf self)
            return;

        if (messenger.IsRegistered<RequestMessage<TMessage>>(self))
            return;

        messenger.Register<TSelf, RequestMessage<TMessage>>(
            self,
            (recipient, message) =>
            {
                var response = recipient.Request(self);
                message.Reply(response);
            }
        );
    }
}
