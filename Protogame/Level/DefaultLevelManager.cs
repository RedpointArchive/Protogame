namespace Protogame
{
    using System.IO;
    using System.Text;

    /// <summary>
    /// The default level manager.
    /// </summary>
    /// <module>Level</module>
    /// <internal>True</internal>
    /// <interface_ref>Protogame.ILevelManager</interface_ref>
    public class DefaultLevelManager : ILevelManager
    {
        /// <summary>
        /// The m_ asset manager.
        /// </summary>
        private readonly IAssetManager m_AssetManager;

        /// <summary>
        /// The m_ reader.
        /// </summary>
        private readonly ILevelReader m_Reader;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultLevelManager"/> class.
        /// </summary>
        /// <param name="reader">
        /// The reader.
        /// </param>
        /// <param name="assetManagerProvider">
        /// The asset manager provider.
        /// </param>
        public DefaultLevelManager(ILevelReader reader, IAssetManagerProvider assetManagerProvider)
        {
            this.m_Reader = reader;
            this.m_AssetManager = assetManagerProvider.GetAssetManager();
        }

        /// <summary>
        /// The load.
        /// </summary>
        /// <param name="world">
        /// The world.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        public void Load(IWorld world, string name)
        {
            var levelAsset = this.m_AssetManager.Get<LevelAsset>(name);
            var levelBytes = Encoding.ASCII.GetBytes(levelAsset.Value);
            using (var stream = new MemoryStream(levelBytes))
            {
                foreach (var entity in this.m_Reader.Read(stream))
                {
                    world.Entities.Add(entity);
                }
            }
        }
    }
}