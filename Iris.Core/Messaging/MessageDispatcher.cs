using Iris.Distributed;
using System;

namespace Iris.Messaging
{
    //TODO: delete, not necessary anymore
    public interface IMessageDispatcher
    {
        void Dispatch<T>(T message) where T : IUserMessage;
    }

    public class MessageDispatcher : IMessageDispatcher
    {
        private readonly IInterprocessMessageBroadcaster _interprocessMessageDispatcher;

        public MessageDispatcher(IInterprocessMessageBroadcaster interprocessMessageDispatcher)
        {
            _interprocessMessageDispatcher = interprocessMessageDispatcher;
        }

        public void Dispatch<T>(T message) where T : IUserMessage
        {
            _interprocessMessageDispatcher.Dispatch(message);
        }
    }
}