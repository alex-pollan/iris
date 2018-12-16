using Microsoft.AspNetCore.Http;

namespace Iris.Messaging
{
    public interface IConnectionRequirement
    {
        bool IsValidConnection(HttpContext context);
        bool ShouldSendMessage(HttpContext context, IUserMessage message);
        object GetDescription(HttpContext context);
    }
}
