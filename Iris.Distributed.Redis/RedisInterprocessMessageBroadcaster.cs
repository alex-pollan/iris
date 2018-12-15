using Iris.Logging;
using Iris.Messaging;
using StackExchange.Redis;
using StackExchange.Redis.Extensions.Core;
using StackExchange.Redis.Extensions.Newtonsoft;
using System.Threading;

namespace Iris.Distributed.Redis
{
    public class RedisInterprocessMessageBroadcaster<T> : IInterprocessMessageBroadcaster<T> where T : IUserMessage
    {
        private readonly IAppRedisConfiguration _configuration;
        private readonly IInterprocessIdentity _interprocessIdentity;
        private readonly ILogger _logger;

        public RedisInterprocessMessageBroadcaster(IAppRedisConfiguration configuration,
            IInterprocessIdentity interprocessIdentity, ILogger logger)
        {
            _configuration = configuration;
            _interprocessIdentity = interprocessIdentity;
            _logger = logger;
        }

        public void Dispatch(T message)
        {
            using (var client = new StackExchangeRedisCacheClient(new NewtonsoftSerializer(), _configuration.ToRedisConfiguration()))
            {
                Log($"Dispatching interprocess message: {message}");

                var count = client.Publish(new RedisChannel(_configuration.Channel, RedisChannel.PatternMode.Literal),
                    new InterprocessMessage<T>
                    {
                        SenderId = _interprocessIdentity.Name,
                        Message = message
                    });

                Log($"Dispatched interprocess message: result = {count}"); 
            }
        }

        private void Log(string message)
        {
            _logger.Log($"RedisInterprocessMessageDispatcher - [Thread ID: {Thread.CurrentThread.ManagedThreadId}] - {message}");
        }
    }
}
