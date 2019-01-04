using Iris.Distributed;
using Iris.Logging;
using Microsoft.AspNetCore.Http;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace Iris.Api.Middleware
{
    public class WebsocketsMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IWebsocketsHandler _handler;
        private readonly IInterprocessMessageReceiver _interProcessMessageReceiver;
        private readonly ILogger _logger;
        private readonly object _lockObj = new object();
        private bool _started = false;

        public WebsocketsMiddleware(RequestDelegate next, IWebsocketsHandler handler,
            IInterprocessMessageReceiver interProcessMessageReceiver,
            ILogger logger)
        {
            _next = next;
            _handler = handler;
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
                if (_started)
                {
                    return;
                }

                _interProcessMessageReceiver.Start();

                _started = true;
            }
        }
    }
}
