using System.IO;

namespace Protogame
{
    public class NullContentCompiler : IContentCompiler
    {
        public byte[] BuildSpriteFont(string fontName, float size, float spacing, bool useKerning)
        {
            return null;
        }
        
        public byte[] BuildTexture2D(Stream source)
        {
            return null;
        }
        
        public byte[] BuildSoundEffect(Stream source)
        {
            return null;
        }
    }
}