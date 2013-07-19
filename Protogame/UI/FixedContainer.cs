using Microsoft.Xna.Framework;
using Protogame;

namespace Protogame
{
    public class FixedContainer : BaseContainer, IContainer
    {
        public Rectangle AbsoluteRectangle { get; set; }

        public FixedContainer(Rectangle absolute)
        {
            this.AbsoluteRectangle = absolute;
        }

        public void Update(ISkin skin, Rectangle layout, GameTime gameTime, ref bool stealFocus)
        {
            if (this.Child != null)
                this.Child.Update(skin, this.AbsoluteRectangle, gameTime, ref stealFocus);
        }

        public void Draw(IRenderContext context, ISkin skin, Rectangle layout)
        {
            skin.DrawFixedContainer(context, layout, this);
            if (this.Child != null)
                this.Child.Draw(context, skin, this.AbsoluteRectangle);
        }
    }
}

