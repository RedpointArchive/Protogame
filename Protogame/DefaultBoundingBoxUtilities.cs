using System;

namespace Protogame
{
    public class DefaultBoundingBoxUtilities : IBoundingBoxUtilities
    {
        public bool Overlaps(params IBoundingBox[] boundingBoxes)
        {
            if (boundingBoxes.Length <= 1)
                return false;
            
            if (boundingBoxes.Length == 2)
            {
                var a = boundingBoxes[0];
                var b = boundingBoxes[1];
                var aX2 = a.X + a.Width;
                var aY2 = a.Y + a.Height;
                var bX2 = b.X + b.Width;
                var bY2 = b.Y + b.Height;
                if (a.X - Math.Abs(a.XSpeed) < bX2 + Math.Abs(b.XSpeed) && aX2 + Math.Abs(a.XSpeed) > b.X - Math.Abs(b.XSpeed) &&
                    a.Y - Math.Abs(a.YSpeed) < bY2 + Math.Abs(b.YSpeed) && aY2 + Math.Abs(a.YSpeed) > b.Y - Math.Abs(b.YSpeed))
                    return true;
            }
            
            foreach (var a in boundingBoxes)
            {
                foreach (var b in boundingBoxes)
                {
                    if (a == b)
                        continue;
                    var aX2 = a.X + a.Width;
                    var aY2 = a.Y + a.Height;
                    var bX2 = b.X + b.Width;
                    var bY2 = b.Y + b.Height;
                    if (a.X - Math.Abs(a.XSpeed) < bX2 + Math.Abs(b.XSpeed) && aX2 + Math.Abs(a.XSpeed) > b.X - Math.Abs(b.XSpeed) &&
                        a.Y - Math.Abs(a.YSpeed) < bY2 + Math.Abs(b.YSpeed) && aY2 + Math.Abs(a.YSpeed) > b.Y - Math.Abs(b.YSpeed))
                        return true;
                }
            }
            return false;
        }
    }
}

