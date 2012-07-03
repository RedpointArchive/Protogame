using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Protogame;

namespace Example.Tiles
{
    [TileName("dirt", TilesetXmlLoader.XmlLoadMode.Tiles)]
    public class DirtTile : Tile
    {
        public DirtTile(Dictionary<string, string> attributes)
        {
            this.Image = "tiles";
            this.X = Convert.ToInt32(attributes["x"]);
            this.Y = Convert.ToInt32(attributes["y"]);
            this.TX = Convert.ToInt32(attributes["tx"]);
            this.TY = Convert.ToInt32(attributes["ty"]);
        }
    }
}
