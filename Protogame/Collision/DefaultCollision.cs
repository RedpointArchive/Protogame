using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Protogame
{
    public class DefaultCollision : ICollision
    {
        private static double EPSILON = 0.000001;

        /// <remarks>
        /// Adapted from http://www.graphics.cornell.edu/pubs/1997/MT97.pdf.
        /// </remarks>
        public Vector3? CollidesWithTriangle(
            Ray ray, 
            Vector3[] trianglePoints,
            out float distance,
            bool testCulling = true)
        {
            if (trianglePoints.Length != 3)
            {
                throw new InvalidOperationException();
            }

            var vert0 = trianglePoints[0];
            var vert1 = trianglePoints[1];
            var vert2 = trianglePoints[2];

            return this.CollidesWithTriangle(ray, vert0, vert1, vert2, out distance, testCulling);
        }

        /// <remarks>
        /// Adapted from http://www.graphics.cornell.edu/pubs/1997/MT97.pdf.
        /// </remarks>
        public Vector3? CollidesWithTriangle(Ray ray, Vector3 vert0, Vector3 vert1, Vector3 vert2, out float distance, bool testCulling = true)
        {
            float t, u, v;

            distance = 0;

            var orig = ray.Position;
            var dir = ray.Direction;

            var edge1 = vert1 - vert0;
            var edge2 = vert2 - vert0;

            var pvec = Vector3.Cross(dir, edge2);

            var det = Vector3.Dot(edge1, pvec);

            if (testCulling)
            {
                if (det < EPSILON)
                {
                    return null;
                }

                var tvec = orig - vert0;

                u = Vector3.Dot(tvec, pvec);
                if (u < 0 || u > det)
                {
                    return null;
                }

                var qvec = Vector3.Cross(tvec, edge1);

                v = Vector3.Dot(dir, qvec);
                if (v < 0 || u + v > det)
                {
                    return null;
                }

                t = Vector3.Dot(edge2, qvec);
                var inv_det = 1 / det;
                t *= inv_det;
                u *= inv_det;
                v *= inv_det;
            }
            else
            {
                if (det > -EPSILON && det < EPSILON)
                {
                    return null;
                }

                var inv_det = 1 / det;

                var tvec = orig - vert0;

                u = Vector3.Dot(tvec, pvec) * inv_det;
                if (u < 0 || u > 1)
                {
                    return null;
                }

                var qvec = Vector3.Cross(tvec, edge1);

                v = Vector3.Dot(dir, qvec) * inv_det;
                if (v < 0 || u + v > 1)
                {
                    return null;
                }

                t = Vector3.Dot(edge2, qvec) * inv_det;
            }

            distance = t;
            return orig + dir * t;
        }
    }
}

