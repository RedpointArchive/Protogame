using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Protogame
{
    public class BoundingBox : IBoundingBox
    {
        public float X { get; private set; }
        public float Y { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public float XSpeed { get; private set; }
        public float YSpeed { get; private set; }

        public BoundingBox(int x, int y, int w, int h)
        {
            this.X = (float)x;
            this.Y = (float)y;
            this.Width = w;
            this.Height = h;
        }

        public static bool Check(IBoundingBox a, IBoundingBox b)
        {
            float aX2 = a.X + a.Width;
            float aY2 = a.Y + a.Height;
            float bX2 = b.X + b.Width;
            float bY2 = b.Y + b.Height;
            return (a.X - Math.Abs(a.XSpeed) < bX2 + Math.Abs(b.XSpeed) && aX2 + Math.Abs(a.XSpeed) > b.X - Math.Abs(b.XSpeed) &&
                    a.Y < bY2 && aY2 > b.Y);
        }
    }
}
