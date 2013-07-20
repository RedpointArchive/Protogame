using Process4;
using Process4.Collections;
using Protogame;
using Ninject;

namespace ProtogameAssetManager
{
    public class NetworkedAssetManagerProvider : IAssetManagerProvider
    {
        private LocalNode m_Node;
        private IKernel m_Kernel;

        public bool IsReady
        {
            get
            {
                try
                {
                    var assetManager = (NetworkAssetManager)
                        new Distributed<NetworkAssetManager>("asset-manager", true);
                    if (assetManager == null)
                        return false;
                    assetManager.SetKernel(this.m_Kernel);
                    return assetManager.IsReady();
                }
                catch (System.Net.Sockets.SocketException)
                {
                    return false;
                }
            }
        }

        public NetworkedAssetManagerProvider(LocalNode node, IKernel kernel)
        {
            this.m_Node = node;
            this.m_Kernel = kernel;
        }

        public IAssetManager GetAssetManager(bool permitCreate = false)
        {
            var assetManager = (NetworkAssetManager)
                (new Distributed<NetworkAssetManager>("asset-manager", !permitCreate));
            assetManager.SetKernel(this.m_Kernel);
            return assetManager;
        }
    }
}

