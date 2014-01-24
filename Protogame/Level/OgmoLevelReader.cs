using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Ninject;
using Ninject.Parameters;

namespace Protogame
{
    public class OgmoLevelReader : ILevelReader
    {
        private IKernel m_Kernel;
    
        public OgmoLevelReader(IKernel kernel)
        {
            this.m_Kernel = kernel;
        }
    
        public IEnumerable<IEntity> Read(Stream stream)
        {
            // FIXME: Store the entities in a tileset so that we 
            // can have pre-rendered tilesets.
            //var tilesetEntity = new DefaultTileset();
            
            // Load the document.
            var doc = XDocument.Load(stream);
            
            // Load the solids.
            var solidsRoot = doc.Root.Element(XName.Get("Solids"));
            var solids = solidsRoot == null ? new XElement[0] : solidsRoot.Elements().Where(x => x.Name.LocalName == "rect");
            
            // Load the tiles.
            var tilesets = from e in doc.Root.Elements()
                           where e.Name.LocalName == "Tiles"
                           select new {
                               Tileset = e.Attribute(XName.Get("tileset")).Value,
                               Tiles = from x in e.Elements()
                                       where x.Name.LocalName == "tile"
                                       select new {
                                           X = Convert.ToSingle(x.Attribute(XName.Get("x")).Value),
                                           Y = Convert.ToSingle(x.Attribute(XName.Get("y")).Value),
                                           TX = Convert.ToInt32(x.Attribute(XName.Get("tx")).Value),
                                           TY = Convert.ToInt32(x.Attribute(XName.Get("ty")).Value)
                                       }
                           };
            
            // Query the kernel to get the classes that
            // implement the required tiles and entities.
            foreach (var solid in solids)
            {
                // TODO: Use Ninject.Extensions.Factory for the solid entity.
                var entity = this.m_Kernel.Get<ISolidEntity>(
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
        }
    }
}
