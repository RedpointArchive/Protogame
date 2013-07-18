using System;
using System.IO;
using System.Text;

namespace Protogame
{
    public class DefaultLevelManager : ILevelManager
    {
        private ILevelReader m_Reader;
        private IAssetManager m_AssetManager;
    
        public DefaultLevelManager(
            ILevelReader reader,
            IAssetManagerProvider assetManagerProvider)
        {
            this.m_Reader = reader;
            this.m_AssetManager = assetManagerProvider.GetAssetManager();
        }

        public void Load(IWorld world, string name)
        {
            var levelAsset = this.m_AssetManager.Get<LevelAsset>(name);
            var levelBytes = Encoding.ASCII.GetBytes(levelAsset.Value);
            using (var stream = new MemoryStream(levelBytes))
            {
                var entities = this.m_Reader.Read(stream);
                world.Entities.AddRange(entities);
            }
        }
    }
}

