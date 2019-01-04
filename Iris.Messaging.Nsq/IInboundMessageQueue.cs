using System;
using System.Collections.Generic;

namespace Iris.Messaging.Nsq
{
    public interface IInboundMessageQueue
    {
        void Start();
        ICollection<Type> MessageTypes { get; }
    }
}
