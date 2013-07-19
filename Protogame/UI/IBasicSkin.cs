using Microsoft.Xna.Framework;

namespace Protogame
{
    public interface IBasicSkin
    {
        Color LightEdgeColor { get; }
        Color SurfaceColor { get; }
        Color DarkSurfaceColor { get; }
        Color BackSurfaceColor { get; }
        Color DarkEdgeColor { get; }
        Color TextColor { get; }
    }
}

