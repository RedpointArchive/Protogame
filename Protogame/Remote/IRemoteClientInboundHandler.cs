using System.Threading.Tasks;

namespace Protogame
{
    public interface IRemoteClientInboundHandler
    {
        Task OnConnected(IRemoteClient remoteClient);

        Task<bool> OnInboundMessage(IRemoteClient remoteClient, string type);
    }
}