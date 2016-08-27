// ReSharper disable CheckNamespace
#pragma warning disable 1591

using System;

namespace Protogame
{
    /// <summary>
    /// The default implementation of <see cref="IBoundingBoxUtilities"/>.
    /// </summary>
    /// <module>Core API</module>
    /// <internal>True</internal>
    /// <interface_ref>Protogame.IBoundingBoxUtilities</interface_ref>
    public class DefaultBoundingBoxUtilities : IBoundingBoxUtilities
    {
        public bool Overlaps(params IBoundingBox[] boundingBoxes)
        {
            if (boundingBoxes.Length <= 1)
            {
                return false;
            }

            if (boundingBoxes.Length == 2)
            {
                var a = boundingBoxes[0];
                var b = boundingBoxes[1];
                if (a == b)
                {
                    // The same bounding box can't collide with itself.
                    return false;
                }

                var aX2 = a.Transform.LocalPosition.X + a.Width;
                var aY2 = a.Transform.LocalPosition.Y + a.Height;
                var aZ2 = a.Transform.LocalPosition.Z + a.Depth;
                var bX2 = b.Transform.LocalPosition.X + b.Width;
                var bY2 = b.Transform.LocalPosition.Y + b.Height;
                var bZ2 = b.Transform.LocalPosition.Z + b.Depth;
                if (a.Width == 0)
                {
                    aX2++;
                }

                if (a.Height == 0)
                {
                    aY2++;
                }

                if (a.Depth == 0)
                {
                    aZ2++;
                }

                if (b.Width == 0)
                {
                    bX2++;
                }

                if (b.Height == 0)
                {
                    bY2++;
                }

                if (b.Depth == 0)
                {
                    bZ2++;
                }

                if (a.Transform.LocalPosition.X - Math.Abs(a.XSpeed) < bX2 + Math.Abs(b.XSpeed)
                    && aX2 + Math.Abs(a.XSpeed) > b.Transform.LocalPosition.X - Math.Abs(b.XSpeed)
                    && a.Transform.LocalPosition.Y - Math.Abs(a.YSpeed) < bY2 + Math.Abs(b.YSpeed)
                    && aY2 + Math.Abs(a.YSpeed) > b.Transform.LocalPosition.Y - Math.Abs(b.YSpeed)
                    && a.Transform.LocalPosition.Z - Math.Abs(a.ZSpeed) < bZ2 + Math.Abs(b.ZSpeed)
                    && aZ2 + Math.Abs(a.ZSpeed) > b.Transform.LocalPosition.Z - Math.Abs(b.ZSpeed))
                {
                    return true;
                }
            }

            foreach (var a in boundingBoxes)
            {
                foreach (var b in boundingBoxes)
                {
                    if (a == b)
                    {
                        continue;
                    }

                    var aX2 = a.Transform.LocalPosition.X + a.Width;
                    var aY2 = a.Transform.LocalPosition.Y + a.Height;
                    var aZ2 = a.Transform.LocalPosition.Z + a.Depth;
                    var bX2 = b.Transform.LocalPosition.X + b.Width;
                    var bY2 = b.Transform.LocalPosition.Y + b.Height;
                    var bZ2 = b.Transform.LocalPosition.Z + b.Depth;
                    if (a.Width == 0)
                    {
                        aX2++;
                    }

                    if (a.Height == 0)
                    {
                        aY2++;
                    }

                    if (a.Depth == 0)
                    {
                        aZ2++;
                    }

                    if (b.Width == 0)
                    {
                        bX2++;
                    }

                    if (b.Height == 0)
                    {
                        bY2++;
                    }

                    if (b.Depth == 0)
                    {
                        bZ2++;
                    }

                    if (a.Transform.LocalPosition.X - Math.Abs(a.XSpeed) < bX2 + Math.Abs(b.XSpeed)
                        && aX2 + Math.Abs(a.XSpeed) > b.Transform.LocalPosition.X - Math.Abs(b.XSpeed)
                        && a.Transform.LocalPosition.Y - Math.Abs(a.YSpeed) < bY2 + Math.Abs(b.YSpeed)
                        && aY2 + Math.Abs(a.YSpeed) > b.Transform.LocalPosition.Y - Math.Abs(b.YSpeed)
                        && a.Transform.LocalPosition.Z - Math.Abs(a.ZSpeed) < bZ2 + Math.Abs(b.ZSpeed)
                        && aZ2 + Math.Abs(a.ZSpeed) > b.Transform.LocalPosition.Z - Math.Abs(b.ZSpeed))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}