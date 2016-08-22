namespace Protogame
{
    using Microsoft.Xna.Framework;
    
    public interface IBasicSkin
    {
        Color BackSurfaceColor { get; }

        Color DarkEdgeColor { get; }

        Color DarkSurfaceColor { get; }

        Color LightEdgeColor { get; }

        Color SurfaceColor { get; }

        Color TextColor { get; }
    }
}