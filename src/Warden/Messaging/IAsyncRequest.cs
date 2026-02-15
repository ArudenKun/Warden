namespace Warden.Messaging;

public interface IAsyncRequest<in TSelf, TMessage>
    where TSelf : class, IAsyncRequest<TSelf, TMessage>
{
    Task<TMessage> RequestAsync(TSelf instance);
}
