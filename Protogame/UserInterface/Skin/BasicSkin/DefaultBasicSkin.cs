namespace Protogame
{
    using Microsoft.Xna.Framework;

    public class DefaultBasicSkin : IBasicSkin
    {
        public Color LightEdgeColor => new Color(160, 160, 160);
        public Color BackSurfaceColor => new Color(96, 96, 96);
        public Color SurfaceColor => new Color(128, 128, 128);
        public Color DarkSurfaceColor => new Color(96, 96, 96);
        public Color DarkEdgeColor => new Color(32, 32, 32);
        public Color TextColor => Color.Black;
    }
}
