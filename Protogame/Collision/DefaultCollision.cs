namespace Protogame
{
    using System;
    using Microsoft.Xna.Framework;

    /// <summary>
    /// The default implementation of <see cref="ICollision"/>.
    /// </summary>
    public class DefaultCollision : ICollision
    {
        /// <summary>
        /// The epsilon value for floating point values.
        /// </summary>
        private const double Epsilon = 0.000001;

        /// <summary>
        /// Detect the location at which the specified ray intersects with the triangle.
        /// </summary>
        /// <remarks>
        /// This calculates whether the ray intersects with the triangle defined by 3 points, and if so, the
        /// precise location at which it intersects.  This method can be used to perform accurate collision detection
        /// against models.
        /// </remarks>
        /// <param name="ray">
        /// The ray to check.
        /// </param>
        /// <param name="trianglePoints">
        /// The 3 points of the triangle.
        /// </param>
        /// <param name="distance">
        /// If the ray collides, this is set to the distance from the ray's position along it's direction
        /// to the collision point.
        /// </param>
        /// <param name="testCulling">
        /// Whether to take culling into account when performing the collision test.
        /// </param>
        /// <returns>
        /// The <see cref="Vector3" /> representing the position at which the collision occurs, or null if there is no collision.
        /// </returns>
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

        /// <summary>
        /// Detect the location at which the specified ray intersects with the triangle.
        /// </summary>
        /// <remarks>
        /// This calculates whether the ray intersects with the triangle defined by 3 points, and if so, the
        /// precise location at which it intersects.  This method can be used to perform accurate collision detection
        /// against models.
        /// </remarks>
        /// <param name="ray">
        /// The ray to check.
        /// </param>
        /// <param name="vert0">
        /// The first vertex of the triangle.
        /// </param>
        /// <param name="vert1">
        /// The second vertex of the triangle.
        /// </param>
        /// <param name="vert2">
        /// The third vertex of the triangle.
        /// </param>
        /// <param name="distance">
        /// If the ray collides, this is set to the distance from the ray's position along it's direction
        /// to the collision point.
        /// </param>
        /// <param name="testCulling">
        /// Whether to take culling into account when performing the collision test.
        /// </param>
        /// <returns>
        /// The <see cref="Vector3" /> representing the position at which the collision occurs, or null if there is no collision.
        /// </returns>
        public Vector3? CollidesWithTriangle(
            Ray ray, 
            Vector3 vert0, 
            Vector3 vert1, 
            Vector3 vert2, 
            out float distance, 
            bool testCulling = true)
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
                if (det < Epsilon)
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
                var invDet = 1 / det;
                t *= invDet;
                u *= invDet;
                v *= invDet;
            }
            else
            {
                if (det > -Epsilon && det < Epsilon)
                {
                    return null;
                }

                var invDet = 1 / det;

                var tvec = orig - vert0;

                u = Vector3.Dot(tvec, pvec) * invDet;
                if (u < 0 || u > 1)
                {
                    return null;
                }

                var qvec = Vector3.Cross(tvec, edge1);

                v = Vector3.Dot(dir, qvec) * invDet;
                if (v < 0 || u + v > 1)
                {
                    return null;
                }

                t = Vector3.Dot(edge2, qvec) * invDet;
            }

            distance = t;

            return orig + (dir * t);
        }
    }
}