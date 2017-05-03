using System;
using System.Threading.Tasks;

namespace Protogame
{
    public interface IRemoteClient : IDisposable
    {
        bool IsConnected { get; }

        Task WaitUntilConnected();

        IRemoteClientReaderWriter TryObtainOutboundReaderWriter();

        Task<IRemoteClientReaderWriter> ObtainOutboundReaderWriter();

        Task<IRemoteClientReaderWriter> ObtainInboundReaderWriter();

        void ReleaseOutboundReaderWriter();
    }
}