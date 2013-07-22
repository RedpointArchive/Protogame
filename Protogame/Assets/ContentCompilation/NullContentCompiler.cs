namespace Protogame
{
    public class NullContentCompiler : IContentCompiler
    {
        public byte[] BuildSpriteFont(string fontName, float size, float spacing, bool useKerning)
        {
            return null;
        }
    }
}