using Microsoft.Xna.Framework;

namespace Protogame
{
    public interface IBasicSkinHelper
    {
        void DrawFlat(IRenderContext context, Rectangle layout);

        void DrawRaised(IRenderContext context, Rectangle layout);

        void DrawSunken(IRenderContext context, Rectangle layout);
    }
}