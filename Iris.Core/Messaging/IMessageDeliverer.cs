using System.Threading.Tasks;

namespace Iris.Messaging
{
    public interface IMessageDeliverer<T> where T : IUserMessage
    {
        Task TryToSend(T message);
    }
}
