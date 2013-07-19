using Process4;
using Process4.Collections;
using Protogame;

namespace ProtogameAssetManager
{
    public class NetworkedAssetManagerProvider : IAssetManagerProvider
    {
        private LocalNode m_Node;

        public bool IsReady
        {
            get
            {
                var assetManager = (NetworkAssetManager)
                    new Distributed<NetworkAssetManager>("asset-manager", true);
                if (assetManager == null)
                    return false;
                return assetManager.IsReady();
            }
        }

        public NetworkedAssetManagerProvider(LocalNode node)
        {
            this.m_Node = node;
        }

        public IAssetManager GetAssetManager(bool permitCreate = false)
        {
            return (NetworkAssetManager)
                (new Distributed<NetworkAssetManager>("asset-manager", !permitCreate));
        }
    }
}

