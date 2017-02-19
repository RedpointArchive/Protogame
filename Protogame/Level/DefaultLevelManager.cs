using System;
using System.Threading.Tasks;
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
            Load(context, levelAsset, null);
        }

        public void Load(object context, LevelAsset levelAsset, Func<IPlan, object, bool> filter)
        {
            var reader = _kernel.Get<ILevelReader>(_currentNode, levelAsset.LevelDataFormat.ToString());
            var levelBytes = Encoding.ASCII.GetBytes(levelAsset.LevelData);

            var node = _kernel.Hierarchy.Lookup(context);
            using (var stream = new MemoryStream(levelBytes))
            {
                foreach (var entity in reader.Read(stream, context, filter))
                {
                    var existingNode = _kernel.Hierarchy.Lookup(entity);
                    if (existingNode != null)
                    {
                        // Remove it from the hierarchy if it's already there.
                        _kernel.Hierarchy.RemoveNode(existingNode);
                    }
                    _kernel.Hierarchy.AddChildNode(node, existingNode);
                }
            }
        }

        public Task LoadAsync(object context, LevelAsset levelAsset)
        {
            Load(context, levelAsset);
            return new Task(() => { });
        }

        public Task LoadAsync(object context, LevelAsset levelAsset, Func<IPlan, object, bool> filter)
        {
            Load(context, levelAsset, filter);
            return new Task(() => { });
        }
    }
}