using Iris.Logging;
using NsqSharp.Bus;

namespace Iris.Messaging.Nsq
{
    public class NsqHandler<T> : IHandleMessages<T> where T : IUserMessage
    {
        private readonly ILogger _logger;
        private readonly IMessageDispatcher _dispatcher;

        public NsqHandler(IMessageDispatcher dispatcher, ILogger logger)
        {
            _logger = logger;
            _dispatcher = dispatcher;
        }

        public void Handle(T message)
        {
            _logger.Log($"NsqHandler - Handling received message: {message}");
            _dispatcher.Dispatch(message);
        }
    }
}
