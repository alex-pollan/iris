using System;

namespace Iris.Distributed
{
    public interface IInterprocessIdentity
    {
        string Name { get; }
    }

    public class MachineNameInterprocessIdentity : IInterprocessIdentity
    {
        public string Name => Environment.MachineName;
    }
}
