namespace Protogame
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml.Linq;
    using Ninject;
    using Ninject.Parameters;

    /// <summary>
    /// The ogmo level reader.
    /// </summary>
    public class OgmoLevelReader : ILevelReader
    {
        /// <summary>
        /// The m_ kernel.
        /// </summary>
        private readonly IKernel m_Kernel;

        /// <summary>
        /// Initializes a new instance of the <see cref="OgmoLevelReader"/> class.
        /// </summary>
        /// <param name="kernel">
        /// The kernel.
        /// </param>
        public OgmoLevelReader(IKernel kernel)
        {
            this.m_Kernel = kernel;
        }

        /// <summary>
        /// The read.
        /// </summary>
        /// <param name="stream">
        /// The stream.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        public IEnumerable<IEntity> Read(Stream stream)
        {
            // FIXME: Store the entities in a tileset so that we 
            // can have pre-rendered tilesets.
            // var tilesetEntity = new DefaultTileset();

            // Load the document.
            var doc = XDocument.Load(stream);

            // Load the solids.
            var solidsRoot = doc.Root.Element(XName.Get("Solids"));
            var solids = solidsRoot == null
                             ? new XElement[0]
                             : solidsRoot.Elements().Where(x => x.Name.LocalName == "rect");

            // Load the tiles.
            var tilesets = from e in doc.Root.Elements()
                           where e.Name.LocalName == "Tiles"
                           select
                               new
                               {
                                   Tileset = e.Attribute(XName.Get("tileset")).Value, 
                                   Tiles = from x in e.Elements()
                                           where x.Name.LocalName == "tile"
                                           select
                                               new
                                               {
                                                   X = Convert.ToSingle(x.Attribute(XName.Get("x")).Value), 
                                                   Y = Convert.ToSingle(x.Attribute(XName.Get("y")).Value), 
                                                   TX = Convert.ToInt32(x.Attribute(XName.Get("tx")).Value), 
                                                   TY = Convert.ToInt32(x.Attribute(XName.Get("ty")).Value)
                                               }
                               };

            // Load the entities.
            var entitydefs = from w in doc.Root.Elements()
                             where w.Name.LocalName == "Entities"
                             from e in w.Descendants()
                             select
                                 new
                                 {
                                     Type = e.Name.LocalName, 
                                     ID = Convert.ToInt32(e.Attribute(XName.Get("id")).Value), 
                                     X = Convert.ToInt32(e.Attribute(XName.Get("x")).Value), 
                                     Y = Convert.ToInt32(e.Attribute(XName.Get("y")).Value), 
                                     Attributes =
                                 e.Attributes().ToDictionary(key => key.Name.LocalName, value => value.Value)
                                 };

            // Query the kernel to get the classes that
            // implement the required tiles and entities.
            foreach (var solid in solids)
            {
                // TODO: Use Ninject.Extensions.Factory for the solid entity.
                var entity =
                    this.m_Kernel.Get<ISolidEntity>(
                        new ConstructorArgument("x", Convert.ToSingle(solid.Attribute(XName.Get("x")).Value)), 
                        new ConstructorArgument("y", Convert.ToSingle(solid.Attribute(XName.Get("y")).Value)), 
                        new ConstructorArgument("width", Convert.ToSingle(solid.Attribute(XName.Get("w")).Value)), 
                        new ConstructorArgument("height", Convert.ToSingle(solid.Attribute(XName.Get("h")).Value)));
                yield return entity;
            }

            foreach (var tileset in tilesets)
            {
                foreach (var tile in tileset.Tiles)
                {
                    var entity = this.m_Kernel.Get<ITileEntity>(
                        tileset.Tileset, 
                        new ConstructorArgument("x", tile.X), 
                        new ConstructorArgument("y", tile.Y), 
                        new ConstructorArgument("tx", tile.TX), 
                        new ConstructorArgument("ty", tile.TY));
                    yield return entity;
                }
            }

            foreach (var entitydef in entitydefs)
            {
                var entity = this.m_Kernel.Get<IEntity>(
                    entitydef.Type, 
                    new ConstructorArgument("name", entitydef.Type), 
                    new ConstructorArgument("id", entitydef.ID), 
                    new ConstructorArgument("x", entitydef.X), 
                    new ConstructorArgument("y", entitydef.Y), 
                    new ConstructorArgument("attributes", entitydef.Attributes));
                yield return entity;
            }
        }
    }
}