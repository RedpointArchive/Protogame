using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Protogame
{
    public interface IEntity
    {
        float X { get; set; }
        float Y { get; set; }
        int Width { get; set; }
        int Height { get; set; }
        string Image { get; set; }
        Color Color { get; set; }
        bool ImageFlipX { get; set; }

        void Update(World world);
        void Draw(World world, XnaGraphics graphics);
    }
}
