namespace Protogame
{
    public interface IBoundingBoxUtilities
    {
        bool Overlaps(params IBoundingBox[] boundingBoxes);
    }
}

