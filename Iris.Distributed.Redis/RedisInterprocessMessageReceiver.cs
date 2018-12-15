using Iris.Logging;
using Iris.Messaging;
using StackExchange.Redis;
using StackExchange.Redis.Extensions.Core;
using StackExchange.Redis.Extensions.Newtonsoft;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Iris.Distributed.Redis
{
    public class RedisInterprocessMessageReceiver<T> : IInterprocessMessageReceiver, IDisposable where T : IUserMessage
    {
        private readonly IAppRedisConfiguration _configuration;
        private readonly IInterprocessIdentity _interprocessIdentity;
        private readonly IMessageDeliverer<T> _messageDeliverer;
        private readonly ILogger _logger;
        private StackExchangeRedisCacheClient _client;

        public RedisInterprocessMessageReceiver(IAppRedisConfiguration configuration,
            IInterprocessIdentity interprocessIdentity,
            IMessageDeliverer<T> messageDeliverer, ILogger logger)
        {
            _configuration = configuration;
            _interprocessIdentity = interprocessIdentity;
            _messageDeliverer = messageDeliverer;
            _logger = logger;
        }

        public void Dispose()
        {
            if (_client == null)
            {
                return;
            }

            _client.UnsubscribeAll();
            Log($"Unsubscribed from Redis channel {_configuration.Channel}");
            _client.Dispose();
            Log($"Redis client disposed");

            _client = null;
        }

        public void Start()
        {
            _client = new StackExchangeRedisCacheClient(new NewtonsoftSerializer(), _configuration.ToRedisConfiguration());

            _client.SubscribeAsync<InterprocessMessage<T>>(new RedisChannel(_configuration.Channel, RedisChannel.PatternMode.Literal),
                OnMesssageReceived);

            Log($"Subscribed to Redis channel {_configuration.Channel}");
        }

        public async Task OnMesssageReceived(InterprocessMessage<T> message)
        {
            Log($"Handling received interprocess message: {message}");

            if (_interprocessIdentity.Name.Equals(message.SenderId))
            {
                Log($"Received interprocess message was sent by myself, ignoring it");

                return;
            }

            await _messageDeliverer.TryToSend(message.Message);
        }

        private void Log(string message)
        {
            _logger.Log($"RedisInterprocessMessageReceiver - [Thread ID: {Thread.CurrentThread.ManagedThreadId}] - {message}");
        }
    }
}
