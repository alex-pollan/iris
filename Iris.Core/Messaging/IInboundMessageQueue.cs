using System;
using System.Collections.Generic;

namespace Iris.Messaging
{
    public interface IInboundMessageQueue
    {
        void Start();
        ICollection<Type> MessageTypes { get; }
    }
}
