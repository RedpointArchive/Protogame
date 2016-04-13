
#if FALSE
using Dx.Runtime;
using Protoinject;
using Protogame;

namespace Protogame
{
    public class NetworkedAssetManagerProvider : IAssetManagerProvider
    {
        private ILocalNode m_Node;
        private IKernel m_Kernel;
        private NetworkAssetManager m_NetworkAssetManager;

        public bool IsReady
        {
            get
            {
                try
                {
                    if (this.m_NetworkAssetManager != null)
                        return this.m_NetworkAssetManager.IsReady();
                    var assetManager = (NetworkAssetManager)
                        new Distributed<NetworkAssetManager>(this.m_Node, "asset-manager", true);
                    if (assetManager == null)
                        return false;
                    this.m_NetworkAssetManager = assetManager;
                    this.m_NetworkAssetManager.SetKernel(this.m_Kernel);
                    return this.m_NetworkAssetManager.IsReady();
                }
                catch (System.Net.Sockets.SocketException)
                {
                    return false;
                }
            }
        }

        public NetworkedAssetManagerProvider(ILocalNode node, IKernel kernel)
        {
            this.m_Node = node;
            this.m_Kernel = kernel;
        }

        public IAssetManager GetAssetManager(bool permitCreate = false)
        {
            if (this.m_NetworkAssetManager != null)
                return this.m_NetworkAssetManager;
            this.m_NetworkAssetManager = (NetworkAssetManager)
                (new Distributed<NetworkAssetManager>(this.m_Node, "asset-manager", !permitCreate));
            this.m_NetworkAssetManager.SetKernel(this.m_Kernel);
            return this.m_NetworkAssetManager;
        }
    }
}

#endif