using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Protogame
{
    public class TreeItem : IContainer
    {
        public IContainer[] Children { get { return new IContainer[0]; } }
        public IContainer Parent { get; set; }
        public int Order { get; set; }
        public virtual string Text { get; set; }
        public bool Focused { get; set; }

        public int Indent
        {
            get { return (this.Text ?? "").Split('.').Length; }
        }

        public void Update(ISkin skin, Rectangle layout, GameTime gameTime, ref bool stealFocus)
        {
            var mouse = Mouse.GetState();
            if (layout.Contains(mouse.X, mouse.Y) && mouse.LeftPressed(this))
                (this.Parent as TreeView).SelectedItem = this;
        }

        public void Draw(IRenderContext context, ISkin skin, Rectangle layout)
        {
            skin.DrawTreeItem(context, layout, this);
        }
    }
}

