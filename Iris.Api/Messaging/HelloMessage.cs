using Iris.Messaging;

namespace Iris.Api.Messaging
{
    public class HelloMessage : IUserMessage
    {
        public string CustomerSet { get; set; }
        public string Text { get; set; }
        public override string ToString()
        {
            return $"HelloMessage(CustomerSet: {CustomerSet}, Text: {Text})";
        }
    }
}
