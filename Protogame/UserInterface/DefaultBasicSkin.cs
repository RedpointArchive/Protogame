namespace Protogame
{
    using Microsoft.Xna.Framework;

    public class DefaultBasicSkin : IBasicSkin
    {
        public Color LightEdgeColor { get { return new Color(160, 160, 160); } }
        public Color BackSurfaceColor { get { return new Color(96, 96, 96); } }
        public Color SurfaceColor { get { return new Color(128, 128, 128); } }
        public Color DarkSurfaceColor { get { return new Color(96, 96, 96); } }
        public Color DarkEdgeColor { get { return new Color(32, 32, 32); } }
        public Color TextColor { get { return Color.Black; } }
    }
}
