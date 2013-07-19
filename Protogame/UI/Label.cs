using Microsoft.Xna.Framework;

namespace Protogame
{
    public class Label : IContainer
    {
        public IContainer[] Children { get { return new IContainer[0]; } }
        public IContainer Parent { get; set; }
        public int Order { get; set; }
        public string Text { get; set; }
        public bool Focused { get; set; }

        public void Update(ISkin skin, Rectangle layout, GameTime gameTime, ref bool stealFocus)
        {
        }

        public void Draw(IRenderContext context, ISkin skin, Rectangle layout)
        {
            skin.DrawLabel(context, layout, this);
        }
    }
}

