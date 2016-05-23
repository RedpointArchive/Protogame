using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Protoinject;

namespace Protogame
{
    /// <summary>
    /// The level reader for levels saved from an ATF level editor.
    /// </summary>
    /// <module>Level</module>
    /// <internal>True</internal>
    /// <interface_ref>Protogame.ILevelReader</interface_ref>
    public class ATFLevelReader : ILevelReader
    {
        private readonly IKernel _kernel;

        private readonly IHierarchy _hierarchy;

        public ATFLevelReader(IKernel kernel, IHierarchy hierarchy)
        {
            _kernel = kernel;
            _hierarchy = hierarchy;
        }

        /// <summary>
        /// Creates the entities from the stream.
        /// </summary>
        /// <param name="stream">The stream which contains an ATF level.</param>
        /// <param name="context">
        /// The context in which entities are being spawned in the hierarchy.  This is
        /// usually the current world, but it doesn't have to be (e.g. if you wanted to
        /// load a level under an entity group, you would pass the entity group here).
        /// </param>
        /// <returns>A list of entities to spawn within the world.</returns>
        public IEnumerable<IEntity> Read(Stream stream, object context)
        {
            var node = _hierarchy.Lookup(context);

            var document = new XmlDocument();
            document.Load(stream);

            // TODO: Implement the level reader.
            return new List<IEntity>();
        }
    }
}