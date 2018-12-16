using Iris.Messaging;

namespace Iris.Distributed
{
    public class InterprocessMessage
    {
        public string SenderId { get; set; }
        public object Message { get; set; }

        public override string ToString()
        {
            return $"InterprocessMessage(SenderId: {SenderId}, Message: {Message})";
        }
    }
}