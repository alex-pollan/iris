using Iris.Logging;
using Iris.Messaging;
using Newtonsoft.Json.Linq;
using StackExchange.Redis;
using StackExchange.Redis.Extensions.Core;
using StackExchange.Redis.Extensions.Newtonsoft;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Iris.Distributed.Redis
{
    public class RedisInterprocessMessageReceiver : IInterprocessMessageReceiver, IDisposable
    {
        private readonly IAppRedisConfiguration _configuration;
        private readonly IInterprocessIdentity _interprocessIdentity;
        private readonly IMessageDeliverer _messageDeliverer;
        private readonly ILogger _logger;
        private StackExchangeRedisCacheClient _client;
        private readonly Dictionary<Type, MessageReceivedHandler> _handlers;

        public RedisInterprocessMessageReceiver(IAppRedisConfiguration configuration,
            IInterprocessIdentity interprocessIdentity,
            IMessageDeliverer messageDeliverer, ILogger logger)
        {
            _configuration = configuration;
            _interprocessIdentity = interprocessIdentity;
            _messageDeliverer = messageDeliverer;
            _logger = logger;
            _client = new StackExchangeRedisCacheClient(new NewtonsoftSerializer(), _configuration.ToRedisConfiguration());
            _handlers = new Dictionary<Type, MessageReceivedHandler>();
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

            _handlers.Clear();

            _client = null;
        }

        public void Start(Type messageType)
        {
            var handler = new MessageReceivedHandler(this, messageType);

            //to ensure uniqueness
            _handlers.Add(messageType, handler);

            _client.SubscribeAsync<InterprocessMessage>(new RedisChannel($"{_configuration.Channel}.{messageType.Name}",
                RedisChannel.PatternMode.Literal), handler.OnMesssageReceived);

            Log($"Subscribed to Redis channel {_configuration.Channel}");
        }

        private void Log(string message)
        {
            _logger.Log($"RedisInterprocessMessageReceiver - [Thread ID: {Thread.CurrentThread.ManagedThreadId}] - {message}");
        }

        public class MessageReceivedHandler
        {
            private RedisInterprocessMessageReceiver _receiver;
            private readonly Type _payloadMessageType;

            public MessageReceivedHandler(RedisInterprocessMessageReceiver receiver, Type payloadMessageType)
            {
                _receiver = receiver;
                _payloadMessageType = payloadMessageType;
            }

            public async Task OnMesssageReceived(InterprocessMessage message)
            {
                _receiver.Log($"Handling received interprocess message: {message}");

                var payloadMessage = ((JObject)message.Message).ToObject(_payloadMessageType);

                await _receiver._messageDeliverer.TryToSend(payloadMessage as IUserMessage);
            }
        }
    }
}
