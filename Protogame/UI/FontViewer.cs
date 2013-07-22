using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Protogame
{
    public class FontViewer : IContainer
    {
        public IContainer[] Children { get { return new IContainer[0]; } }
        public IContainer Parent { get; set; }
        public int Order { get; set; }
        public bool Focused { get; set; }
        public FontAsset Font { get; set; }

        public FontViewer()
        {
            this.Font = null;
        }

        public void Update(ISkin skin, Rectangle layout, GameTime gameTime, ref bool stealFocus)
        {
        }

        public void Draw(IRenderContext context, ISkin skin, Rectangle layout)
        {
            skin.DrawFontViewer(context, layout, this);
        }
    }
}

