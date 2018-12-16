using System;

namespace Iris.Distributed
{
    public interface IInterprocessMessageReceiver
    {
        void Start(Type messageType);
    }
}