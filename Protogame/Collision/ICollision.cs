using Microsoft.Xna.Framework;

namespace Protogame
{
    public interface ICollision
    {
        Vector3? CollidesWithTriangle(Ray ray, Vector3[] trianglePoints, out float distance, bool testCulling = true);

        Vector3? CollidesWithTriangle(Ray ray, Vector3 vert0, Vector3 vert1, Vector3 vert2, out float distance, bool testCulling = true);
    }
}

