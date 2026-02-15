using Autofac.Core.Resolving.Pipeline;
using CommunityToolkit.Mvvm.Messaging;
using ServiceScan.SourceGenerator;

namespace Warden.Dependency.Middlewares;

public sealed partial class MessengerRecipientMiddleware : IResolveMiddleware
{
    public void Execute(ResolveRequestContext context, Action<ResolveRequestContext> next)
    {
        next(context);
        if (!context.NewInstanceActivated)
            return;

        var instance = context.Instance!;
        RegisterRecipient(instance, null);
    }

    public PipelinePhase Phase => PipelinePhase.Activation;

    [GenerateServiceRegistrations(
        AssignableTo = typeof(IRecipient<>),
        CustomHandler = nameof(RegisterRecipientHandler)
    )]
    public static partial void RegisterRecipient(object instance, IMessenger? messenger);

    private static void RegisterRecipientHandler<TRecipient, TMessage>(
        object instance,
        IMessenger? messenger
    )
        where TRecipient : class, IRecipient<TMessage>
        where TMessage : class
    {
        messenger ??= WeakReferenceMessenger.Default;
        if (instance is not TRecipient recipient)
            return;

        if (messenger.IsRegistered<TMessage>(instance))
            return;

        messenger.Register(recipient);
    }
}
