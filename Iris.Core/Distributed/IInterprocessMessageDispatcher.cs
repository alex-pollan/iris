using Iris.Messaging;

namespace Iris.Distributed
{
    public interface IInterprocessMessageDispatcher<T> where T : IUserMessage
    {
        void Dispatch(T message);
    }
}
