using Iris.Distributed;

namespace Iris.Messaging
{
    public interface IMessageDispatcher<T> where T : IUserMessage
    {
        void Dispatch(T message);
    }

    public class MessageDispatcher<T> : IMessageDispatcher<T> where T : IUserMessage
    {
        private readonly IMessageDeliverer<T> _messageDeliverer;
        private readonly IInterprocessMessageDispatcher<T> _interprocessMessageDispatcher;

        public MessageDispatcher(IMessageDeliverer<T> messageDeliverer, IInterprocessMessageDispatcher<T> interprocessMessageDispatcher)
        {
            _messageDeliverer = messageDeliverer;
            _interprocessMessageDispatcher = interprocessMessageDispatcher;
        }

        public void Dispatch(T message)
        {
            _messageDeliverer.TryToSend(message);
            _interprocessMessageDispatcher.Dispatch(message);
        }
    }
}