using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Protogame
{
    public class Tile : IBoundingBox
    {
        public string Image { get; set; }
        public Color Color { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int TX { get; set; }
        public int TY { get; set; }
        public float XSpeed { get { return 0; } }
        public float YSpeed { get { return 0; } }

        protected Tile()
        {
            this.Width = Tileset.TILESET_CELL_WIDTH;
            this.Height = Tileset.TILESET_CELL_HEIGHT;
            this.TX = -1;
            this.TY = -1;
            this.Color = Color.White;
        }
    }
}
