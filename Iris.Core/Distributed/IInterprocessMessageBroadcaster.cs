using Iris.Messaging;

namespace Iris.Distributed
{
    public interface IInterprocessMessageBroadcaster<T> where T : IUserMessage
    {
        void Dispatch(T message);
    }
}
