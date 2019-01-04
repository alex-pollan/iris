using Iris.Messaging;
using Microsoft.AspNetCore.Http;
using System;

namespace Iris.Samples.ApiController.Messaging
{
    public class CustomerSetConnectionRequirement : IConnectionRequirement
    {
        public object GetDescription(HttpContext context)
        {
            return context.Request.Query["customerSet"];
        }

        public bool IsValidConnection(HttpContext context)
        {
            return context.Request.Query["customerSet"].Count > 0;
        }

        public bool ShouldSendMessage(HttpContext context, IUserMessage message)
        {
            return (message as HelloMessage).CustomerSet.Equals(context.Request.Query["customerSet"], StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
