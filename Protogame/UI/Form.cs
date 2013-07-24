using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Input;

namespace Protogame
{
    public class Form : IContainer
    {
        public IContainer[] Children { get { return this.m_Controls.Select(x => x.Value).ToArray(); } }
        public IContainer Parent { get; set; }
        public int Order { get; set; }
        public bool Focused { get; set; }
        public int LabelMaxWidth { get; set; }
        private Dictionary<IContainer, IContainer> m_Controls = new Dictionary<IContainer, IContainer>();
        
        public Form()
        {
            this.LabelMaxWidth = 200;
        }
        
        public void AddControl(string name, IContainer control)
        {
            this.m_Controls.Add(new Label { Text = name, Parent = this }, control);
            control.Parent = this;
        }
        
        public void AddControl(IContainer name, IContainer control)
        {
            this.m_Controls.Add(name, control);
            control.Parent = this;
        }
        
        public IEnumerable<KeyValuePair<IContainer, Rectangle>> LabelsWithLayouts(Rectangle layout)
        {
            var i = 0;
            foreach (var kv in this.m_Controls)
            {
                yield return new KeyValuePair<IContainer, Rectangle>(
                    kv.Key,
                    new Rectangle(
                        layout.X,
                        layout.Y + i * 20,
                        Math.Min(layout.Width / 2, this.LabelMaxWidth),
                        20));
                i++;
            }
        }
        
        public IEnumerable<KeyValuePair<IContainer, Rectangle>> ChildrenWithLayouts(Rectangle layout)
        {
            var i = 0;
            foreach (var kv in this.m_Controls)
            {
                yield return new KeyValuePair<IContainer, Rectangle>(
                    kv.Value,
                    new Rectangle(
                        layout.X + Math.Min(layout.Width / 2, this.LabelMaxWidth),
                        layout.Y + i * 20,
                        layout.Width - Math.Min(layout.Width / 2, this.LabelMaxWidth),
                        20));
                i++;
            }
        }
    
        public void Update(ISkin skin, Rectangle layout, GameTime gameTime, ref bool stealFocus)
        {
            var mouse = Mouse.GetState();
            if (mouse.LeftPressed(this.GetHashCode()))
                this.Focus();
            foreach (var kv in this.LabelsWithLayouts(layout))
            {
                kv.Key.Update(skin, kv.Value, gameTime, ref stealFocus);
                if (stealFocus)
                    return;
            }
            foreach (var kv in this.ChildrenWithLayouts(layout))
            {
                kv.Key.Update(skin, kv.Value, gameTime, ref stealFocus);
                if (stealFocus)
                    return;
            }
        }

        public void Draw(IRenderContext context, ISkin skin, Rectangle layout)
        {
            skin.DrawForm(context, layout, this);
            foreach (var kv in this.LabelsWithLayouts(layout))
                kv.Key.Draw(context, skin, kv.Value);
            foreach (var kv in this.ChildrenWithLayouts(layout))
                kv.Key.Draw(context, skin, kv.Value);
        }
    }
}

