using Iris.Logging;
using Iris.Messaging;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Iris.Api.Middleware
{
    public interface IWebsocketsHandler<T> where T : IUserMessage
    {
        Task Register(HttpContext context, WebSocket webSocket);
    }

    public class WebsocketsHandler<T> : IWebsocketsHandler<T>, IMessageDeliverer<T> where T : IUserMessage
    {
        private ConcurrentDictionary<string, Channel> _channels;
        private readonly ILogger _logger;
        private readonly IConnectionRequirement<T> _connectionRequirement;

        public WebsocketsHandler(IConnectionRequirement<T> connectionRequirement, ILogger logger)
        {
            _channels = new ConcurrentDictionary<string, Channel>();
            _logger = logger;
            _connectionRequirement = connectionRequirement;
        }

        public async Task Register(HttpContext context, WebSocket webSocket)
        {
            if (!_connectionRequirement.IsValidConnection(context))
            {
                return;
            }

            _channels.TryAdd(context.Connection.Id, new Channel { Context = context, Websocket = webSocket });
            _logger.Log($"WebsocketsHandler - Registered new channel: {_connectionRequirement.GetDescription(context)}");

            await WaitForWebsocketToClose(webSocket);

            _logger.Log($"WebsocketsHandler - Removing closed channel: {_connectionRequirement.GetDescription(context)}");
            _channels.TryRemove(context.Connection.Id, out Channel removed);
        }

        public async Task TryToSend(T message)
        {
            _logger.Log($"WebsocketsHandler - Trying to send message: {message}");

            foreach (var channel in _channels.Values)
            {
                if (_connectionRequirement.ShouldSendMessage(channel.Context, message))
                {
                    var bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
                    await channel.Websocket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true,
                        CancellationToken.None);
                    _logger.Log($"WebsocketsHandler - Message sent to websocket: {message}");
                }
            }
        }

        private static async Task WaitForWebsocketToClose(WebSocket webSocket)
        {
            var buffer = new byte[1024 * 4];
            WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            while (!result.CloseStatus.HasValue)
            {
                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }
            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        }
    }
}
