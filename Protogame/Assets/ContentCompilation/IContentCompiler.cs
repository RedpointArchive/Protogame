#if PLATFORM_WINDOWS || PLATFORM_MAC || PLATFORM_LINUX

using System.IO;

namespace Protogame
{
    public interface IContentCompiler
    {
        byte[] BuildSpriteFont(string fontName, float size, float spacing, bool useKerning);
        byte[] BuildTexture2D(Stream source);
        byte[] BuildSoundEffect(Stream source);
    }
}

#endif