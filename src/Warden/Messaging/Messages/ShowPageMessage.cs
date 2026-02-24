using Warden.ViewModels;

namespace Warden.Messaging.Messages;

public sealed record ShowPageMessage<TViewModel>() : ShowPageMessage(typeof(TViewModel))
    where TViewModel : ViewModel;

public record ShowPageMessage(Type ViewModelType)
{
    public static implicit operator ShowPageMessage(Type type) => new(type);

    public static implicit operator Type(ShowPageMessage message) => message.ViewModelType;
}
