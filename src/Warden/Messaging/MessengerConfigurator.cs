using CommunityToolkit.Mvvm.Messaging;
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
}
