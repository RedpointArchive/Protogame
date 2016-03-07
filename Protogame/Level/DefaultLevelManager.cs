using Protoinject;

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
        /// The dependency injection kernel.
        /// </summary>
        private readonly IKernel _kernel;

        /// <summary>
        /// The m_ reader.
        /// </summary>
        private readonly ILevelReader m_Reader;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultLevelManager"/> class.
        /// </summary>
        /// <param name="kernel">
        /// The dependency injection kernel.
        /// </param>
        /// <param name="reader">
        /// The reader.
        /// </param>
        /// <param name="assetManagerProvider">
        /// The asset manager provider.
        /// </param>
        public DefaultLevelManager(IKernel kernel, ILevelReader reader, IAssetManagerProvider assetManagerProvider)
        {
            _kernel = kernel;
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
            var worldNode = _kernel.Hierarchy.Lookup(world);
            // TODO: This doesn't work right yet because the collision system still relies on
            // checking the bounding boxes of entities, rather than entities having bounding box
            // components or something of that nature.
            //var levelGroup = _kernel.Get<EntityGroup>(worldNode, null, "LevelEntities", new IInjectionAttribute[0]);
            //var levelNode = _kernel.Hierarchy.Lookup(levelGroup);
            using (var stream = new MemoryStream(levelBytes))
            {
                foreach (var entity in this.m_Reader.Read(stream))
                {
                    var existingNode = _kernel.Hierarchy.Lookup(entity);
                    if (existingNode != null)
                    {
                        // Remove it from the hierarchy if it's already there.
                        _kernel.Hierarchy.RemoveNode(existingNode);
                    }
                    _kernel.Hierarchy.AddChildNode(worldNode, _kernel.Hierarchy.CreateNodeForObject(entity));
                }
            }
        }
    }
}