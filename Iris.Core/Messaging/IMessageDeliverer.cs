using System.Threading.Tasks;

namespace Iris.Messaging
{
    public interface IMessageDeliverer
    {
        Task TryToSend(IUserMessage message);
    }
}
