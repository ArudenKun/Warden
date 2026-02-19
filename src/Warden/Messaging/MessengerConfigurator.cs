using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using ServiceScan.SourceGenerator;

namespace Warden.Messaging;

public static partial class MessengerConfigurator
{
    [GenerateServiceRegistrations(
        AssignableTo = typeof(IRecipient<>),
        CustomHandler = nameof(RegisterRecipientHandler)
    )]
    public static partial void RegisterRecipient(object instance, IMessenger? messenger = null);

    private static void RegisterRecipientHandler<T, TMessage>(
        object instance,
        IMessenger? messenger
    )
        where T : class, IRecipient<TMessage>
        where TMessage : class
    {
        messenger ??= WeakReferenceMessenger.Default;
        if (instance is T recipient)
        {
            messenger.Register(recipient);
        }
    }

    [GenerateServiceRegistrations(
        AssignableTo = typeof(IRequest<,>),
        CustomHandler = nameof(RegisterRequestHandler)
    )]
    public static partial void RegisterRequest(object instance, IMessenger? messenger = null);

    private static void RegisterRequestHandler<TSelf, TMessage>(
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
