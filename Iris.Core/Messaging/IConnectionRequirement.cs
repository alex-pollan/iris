using Microsoft.AspNetCore.Http;

namespace Iris.Messaging
{
    public interface IConnectionRequirement<T> where T : IUserMessage
    {
        bool IsValidConnection(HttpContext context);
        bool ShouldSendMessage(HttpContext context, T message);
        object GetDescription(HttpContext context);
    }
}
