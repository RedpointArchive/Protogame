using System;
using System.Collections.Generic;

#if LEGACY

namespace Protogame
{
    [TileName("rect", TilesetXmlLoader.XmlLoadMode.Solids)]
    public class SolidTile : Tile, ISolid
    {
        public SolidTile(Dictionary<string, string> attributes)
        {
            this.Image = null;
            this.X = Convert.ToInt32(attributes["x"]);
            this.Y = Convert.ToInt32(attributes["y"]);
            this.Width = Convert.ToInt32(attributes["w"]);
            this.Height = Convert.ToInt32(attributes["h"]);
        }
    }
}

#endif