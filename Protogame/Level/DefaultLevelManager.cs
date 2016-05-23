using System;
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
        private readonly IKernel _kernel;
        private readonly INode _currentNode;

        public DefaultLevelManager(IKernel kernel, INode currentNode)
        {
            _kernel = kernel;
            _currentNode = currentNode;
        }

        /// <summary>
        /// Loads a level entity into the game hierarchy, with the specified
        /// context as the place to load entities.  Normally you'll pass in the
        /// game world here, but you don't have to.  For example, if you wanted to
        /// load the level into an entity group, you would pass the entity group
        /// as the context instead.
        /// </summary>
        /// <param name="context">Usually the current game world, but can be any object in the hierarchy.</param>
        /// <param name="levelAsset">The level to load.</param>
        public void Load(object context, LevelAsset levelAsset)
        {
            var reader = _kernel.Get<ILevelReader>(_currentNode, levelAsset.LevelDataFormat.ToString());
            var levelBytes = Encoding.ASCII.GetBytes(levelAsset.LevelData);

            var node = _kernel.Hierarchy.Lookup(context);
            using (var stream = new MemoryStream(levelBytes))
            {
                foreach (var entity in reader.Read(stream, node))
                {
                    var existingNode = _kernel.Hierarchy.Lookup(entity);
                    if (existingNode != null)
                    {
                        // Remove it from the hierarchy if it's already there.
                        _kernel.Hierarchy.RemoveNode(existingNode);
                    }
                    _kernel.Hierarchy.AddChildNode(node, _kernel.Hierarchy.CreateNodeForObject(entity));
                }
            }
        }

        [Obsolete("Use one of the other Load methods instead.")]
        public void Load(IWorld world, string name)
        {
            // This legacy method only accepts a name, so lazy load the asset manager
            // to support this old behaviour.
            var assetManager = _kernel.Get<IAssetManagerProvider>(_currentNode).GetAssetManager();
            var levelAsset = assetManager.Get<LevelAsset>(name);

            if (levelAsset.LevelDataFormat != LevelDataFormat.OgmoEditor)
            {
                throw new NotSupportedException(
                    "This method is only for legacy usage, and only " +
                    "supports the Ogmo Editor level format.");
            }

            var levelReader = _kernel.Get<ILevelReader>(_currentNode, LevelDataFormat.OgmoEditor.ToString());
            var levelBytes = Encoding.ASCII.GetBytes(levelAsset.LevelData);

            var worldNode = _kernel.Hierarchy.Lookup(world);
            // TODO: This doesn't work right yet because the collision system still relies on
            // checking the bounding boxes of entities, rather than entities having bounding box
            // components or something of that nature.
            //var levelGroup = _kernel.Get<EntityGroup>(worldNode, null, "LevelEntities", new IInjectionAttribute[0]);
            //var levelNode = _kernel.Hierarchy.Lookup(levelGroup);
            using (var stream = new MemoryStream(levelBytes))
            {
                foreach (var entity in levelReader.Read(stream, _currentNode))
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