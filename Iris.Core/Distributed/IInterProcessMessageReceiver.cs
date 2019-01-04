using System;

namespace Iris.Distributed
{
    public interface IInterprocessMessageReceiver
    {
        void RegisterMessageType(Type messageType);
        void Start();
    }
}