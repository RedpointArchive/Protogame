using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Protogame;

namespace Example.RTS.Tiles
{
    [TileName("SurfaceTiles", TilesetXmlLoader.XmlLoadMode.Tiles)]
    public class SurfaceTile : Tile
    {
        public SurfaceTile(Dictionary<string, string> attributes)
        {
            this.Image = "SurfaceTiles";
            this.X = Convert.ToInt32(attributes["x"]);
            this.Y = Convert.ToInt32(attributes["y"]);
            this.TX = Convert.ToInt32(attributes["tx"]);
            this.TY = Convert.ToInt32(attributes["ty"]);
        }
    }
}
