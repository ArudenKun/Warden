namespace Warden.Messaging;

public interface IRequest<in TSelf, out TMessage>
    where TSelf : class, IRequest<TSelf, TMessage>
{
    TMessage Request(TSelf instance);
}
