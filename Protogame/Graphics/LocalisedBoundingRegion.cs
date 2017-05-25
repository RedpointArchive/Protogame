using Microsoft.Xna.Framework;

namespace Protogame
{
    public class LocalisedBoundingRegion
    {
        public LocalisedBoundingRegion(float radius)
        {
            Type = LocalisedBoundingRegionType.Sphere;
            Radius = radius;
        }

        public LocalisedBoundingRegion(Vector3 relativeOrigin, Vector3 size)
        {
            Type = LocalisedBoundingRegionType.Box;
            RelativeOrigin = relativeOrigin;
            Size = size;
        }

        public LocalisedBoundingRegion(Vector3 rayVector)
        {
            Type = LocalisedBoundingRegionType.Ray;
            RayVector = rayVector;
        }

        public bool Intersects(BoundingFrustum frustrum, Vector3 absolutePosition)
        {
            switch (Type)
            {
                case LocalisedBoundingRegionType.Sphere:
                    return frustrum.Intersects(new BoundingSphere(absolutePosition, Radius));
                case LocalisedBoundingRegionType.Box:
                    return frustrum.Intersects(new Microsoft.Xna.Framework.BoundingBox(absolutePosition - RelativeOrigin, absolutePosition - RelativeOrigin + Size));
                case LocalisedBoundingRegionType.Ray:
                    return frustrum.Intersects(new Ray(absolutePosition, RayVector)) != null;
            }

            return false;
        }
        
        public LocalisedBoundingRegionType Type { get; set; }

        public float Radius { get; set; }

        public Vector3 RelativeOrigin { get; private set; }

        public Vector3 Size { get; private set; }

        public Vector3 RayVector { get; private set; }
    }
}
