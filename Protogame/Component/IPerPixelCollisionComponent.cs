using Microsoft.Xna.Framework;

namespace Protogame
{
    public interface IPerPixelCollisionComponent
    {
        IAssetReference<TextureAsset> Texture { get; set; }

        Vector2? RotationAnchor { get; set; }

        int GetPixelWidth();

        int GetPixelHeight();

        Color[] GetPixelData();
    }
}
