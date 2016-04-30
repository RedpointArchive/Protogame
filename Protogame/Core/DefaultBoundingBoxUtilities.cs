namespace Protogame
{
    using System;

    /// <summary>
    /// The default bounding box utilities.
    /// </summary>
    public class DefaultBoundingBoxUtilities : IBoundingBoxUtilities
    {
        /// <summary>
        /// The overlaps.
        /// </summary>
        /// <param name="boundingBoxes">
        /// The bounding boxes.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
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

                var aX2 = a.LocalMatrix.Translation.X + a.Width;
                var aY2 = a.LocalMatrix.Translation.Y + a.Height;
                var aZ2 = a.LocalMatrix.Translation.Z + a.Depth;
                var bX2 = b.LocalMatrix.Translation.X + b.Width;
                var bY2 = b.LocalMatrix.Translation.Y + b.Height;
                var bZ2 = b.LocalMatrix.Translation.Z + b.Depth;
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

                if (a.LocalMatrix.Translation.X - Math.Abs(a.XSpeed) < bX2 + Math.Abs(b.XSpeed)
                    && aX2 + Math.Abs(a.XSpeed) > b.LocalMatrix.Translation.X - Math.Abs(b.XSpeed)
                    && a.LocalMatrix.Translation.Y - Math.Abs(a.YSpeed) < bY2 + Math.Abs(b.YSpeed)
                    && aY2 + Math.Abs(a.YSpeed) > b.LocalMatrix.Translation.Y - Math.Abs(b.YSpeed)
                    && a.LocalMatrix.Translation.Z - Math.Abs(a.ZSpeed) < bZ2 + Math.Abs(b.ZSpeed)
                    && aZ2 + Math.Abs(a.ZSpeed) > b.LocalMatrix.Translation.Z - Math.Abs(b.ZSpeed))
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

                    var aX2 = a.LocalMatrix.Translation.X + a.Width;
                    var aY2 = a.LocalMatrix.Translation.Y + a.Height;
                    var aZ2 = a.LocalMatrix.Translation.Z + a.Depth;
                    var bX2 = b.LocalMatrix.Translation.X + b.Width;
                    var bY2 = b.LocalMatrix.Translation.Y + b.Height;
                    var bZ2 = b.LocalMatrix.Translation.Z + b.Depth;
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

                    if (a.LocalMatrix.Translation.X - Math.Abs(a.XSpeed) < bX2 + Math.Abs(b.XSpeed)
                        && aX2 + Math.Abs(a.XSpeed) > b.LocalMatrix.Translation.X - Math.Abs(b.XSpeed)
                        && a.LocalMatrix.Translation.Y - Math.Abs(a.YSpeed) < bY2 + Math.Abs(b.YSpeed)
                        && aY2 + Math.Abs(a.YSpeed) > b.LocalMatrix.Translation.Y - Math.Abs(b.YSpeed)
                        && a.LocalMatrix.Translation.Z - Math.Abs(a.ZSpeed) < bZ2 + Math.Abs(b.ZSpeed)
                        && aZ2 + Math.Abs(a.ZSpeed) > b.LocalMatrix.Translation.Z - Math.Abs(b.ZSpeed))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}