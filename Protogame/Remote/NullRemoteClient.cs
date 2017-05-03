using System;
using System.Threading.Tasks;

namespace Protogame
{
    public class NullRemoteClient : IRemoteClient
    {
        public bool IsConnected => false;

        public void Dispose()
        {
        }

        public async Task<IRemoteClientReaderWriter> ObtainInboundReaderWriter()
        {
            throw new NotSupportedException();
        }

        public async Task<IRemoteClientReaderWriter> ObtainOutboundReaderWriter()
        {
            throw new NotSupportedException();
        }

        public void ReleaseOutboundReaderWriter()
        {
        }

        public IRemoteClientReaderWriter TryObtainOutboundReaderWriter()
        {
            return null;
        }

        public async Task WaitUntilConnected()
        {
        }
    }
}