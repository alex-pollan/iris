using Microsoft.AspNetCore.Http;
using System.Net.WebSockets;

namespace Iris.Api.Middleware
{
    public class Channel
    {
        public WebSocket Websocket { get; set; }
        public HttpContext Context { get; set; }
    }
}
