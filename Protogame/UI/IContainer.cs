using Microsoft.Xna.Framework;

namespace Protogame
{
    public interface IContainer
    {
        IContainer[] Children { get; }
        IContainer Parent { get; set; }
        int Order { get; set; }
        bool Focused { get; set; }

        void Update(ISkin skin, Rectangle layout, GameTime gameTime, ref bool stealFocus);
        void Draw(IRenderContext context, ISkin skin, Rectangle layout);
    }
}
