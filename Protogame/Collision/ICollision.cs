using Microsoft.Xna.Framework;

namespace Protogame
{    
    /// <summary>
    /// A service that provides advanced, but general purpose collision detection methods.
    /// </summary>
    /// <remarks>
    /// Most MonoGame classes such as <see cref="Rectangle"/> and <see cref="Ray" /> already ship with
    /// intersection methods.  This service provides advanced collision detection methods such as
    /// ray collisions with triangles.
    /// </remarks>
    /// <module>Collision</module>
    public interface ICollision
    {
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
        Vector3? CollidesWithTriangle(Ray ray, Vector3[] trianglePoints, out float distance, bool testCulling = true);

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
        Vector3? CollidesWithTriangle(
            Ray ray, 
            Vector3 vert0, 
            Vector3 vert1, 
            Vector3 vert2, 
            out float distance, 
            bool testCulling = true);
    }
}