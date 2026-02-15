namespace Warden.Messaging;

public interface ICollectionRequest<in TSelf, TMessage>
    where TSelf : class, ICollectionRequest<TSelf, TMessage>
{
    ICollection<TMessage> Request(TSelf instance);
}
