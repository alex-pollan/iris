using Iris.Distributed;
using Iris.Logging;
using Iris.Messaging;
using Microsoft.AspNetCore.Http;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace Iris.Api.Middleware
{
    public class WebsocketsMiddleware 
    {
        private readonly RequestDelegate _next;
        private readonly IWebsocketsHandler _handler;
        private readonly IInboundMessageQueue _inboundMessageQueue;
        private readonly IInterprocessMessageReceiver _interProcessMessageReceiver;
        private readonly ILogger _logger;
        private readonly object _lockObj = new object();
        private bool _busStarted = false;

        public WebsocketsMiddleware(RequestDelegate next, IWebsocketsHandler handler,
            IInboundMessageQueue inboundMessageQueue, IInterprocessMessageReceiver interProcessMessageReceiver,
            ILogger logger)
        {
            _next = next;
            _handler = handler;
            _inboundMessageQueue = inboundMessageQueue;
            _interProcessMessageReceiver = interProcessMessageReceiver;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path == "/ws")
            {
                if (context.WebSockets.IsWebSocketRequest)
                {
                    Start();

                    WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
                    await _handler.Register(context, webSocket);
                }
                else
                {
                    context.Response.StatusCode = 400;
                }
            }
            else
            {
                await _next(context);
            }
        }

        private void Start()
        {
            lock (_lockObj)
            {
                if (_busStarted)
                {
                    return;
                }

                _inboundMessageQueue.Start();

                foreach (var messageType in _inboundMessageQueue.MessageTypes)
                {
                    _interProcessMessageReceiver.Start(messageType);
                }

                _busStarted = true;
            }
        }
    }
}
