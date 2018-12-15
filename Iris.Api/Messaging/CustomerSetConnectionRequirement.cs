using Iris.Messaging;
using Microsoft.AspNetCore.Http;
using System;

namespace Iris.Api.Messaging
{
    public class CustomerSetConnectionRequirement<T> : IConnectionRequirement<T> where T : IUserMessage
    {
        public object GetDescription(HttpContext context)
        {
            return context.Request.Query["customerSet"];
        }

        public bool IsValidConnection(HttpContext context)
        {
            return context.Request.Query["customerSet"].Count > 0;
        }

        public bool ShouldSendMessage(HttpContext context, T message)
        {
            return (message as HelloMessage).CustomerSet.Equals(context.Request.Query["customerSet"], StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
