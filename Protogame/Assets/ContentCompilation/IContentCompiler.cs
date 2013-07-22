#if PLATFORM_WINDOWS || PLATFORM_MAC || PLATFORM_LINUX

using Microsoft.Xna.Framework.Content.Pipeline.Graphics;

namespace Protogame
{
    public interface IContentCompiler
    {
        byte[] BuildSpriteFont(string fontName, float size, float spacing, bool useKerning);
    }
}

#endif