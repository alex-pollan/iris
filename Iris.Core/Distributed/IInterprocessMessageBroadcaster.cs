using Iris.Messaging;

namespace Iris.Distributed
{
    public interface IInterprocessMessageBroadcaster
    {
        void Dispatch<T>(T message) where T : IUserMessage;
    }
}
