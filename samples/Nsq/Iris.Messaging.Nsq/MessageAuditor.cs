using NsqSharp.Bus;
using NsqSharp.Bus.Logging;
using System;

namespace Iris.Messaging.Nsq
{
    public class MessageAuditor : IMessageAuditor
    {
        public void OnReceived(IBus bus, IMessageInformation info) { }
        public void OnSucceeded(IBus bus, IMessageInformation info) { }
        public void OnFailed(IBus bus, IFailedMessageInformation failedInfo)
        {
            Console.Error?.WriteLine(failedInfo.Exception.ToString());
        }
    }
}
