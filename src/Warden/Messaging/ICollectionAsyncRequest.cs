namespace Warden.Messaging;

public interface ICollectionAsyncRequest<in TSelf, TMessage>
    where TSelf : class, ICollectionAsyncRequest<TSelf, TMessage>
{
    Task<ICollection<TMessage>> Request(TSelf instance);
}
