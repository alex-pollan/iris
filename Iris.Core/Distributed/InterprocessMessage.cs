using Iris.Messaging;

namespace Iris.Distributed
{
    public class InterprocessMessage<T> where T : IUserMessage
    {
        public string SenderId { get; set; }
        public T Message { get; set; }

        public override string ToString()
        {
            return $"InterprocessMessage(SenderId: {SenderId}, Message: {Message})";
        }
    }
}