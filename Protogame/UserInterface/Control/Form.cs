using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace Protogame
{
    public class Form : IContainer
    {
        private readonly Dictionary<IContainer, IContainer> _controls = new Dictionary<IContainer, IContainer>();
        
        public Form()
        {
            LabelMaxWidth = 200;
        }
        
        public IContainer[] Children
        {
            get
            {
                return _controls.Select(x => x.Value).ToArray();
            }
        }
        
        public bool Focused { get; set; }
        
        public int LabelMaxWidth { get; set; }
        
        public int Order { get; set; }
        
        public IContainer Parent { get; set; }

        public object Userdata { get; set; }
        
        public void AddControl(string name, IContainer control)
        {
            _controls.Add(new Label { Text = name, Parent = this }, control);
            control.Parent = this;
        }
        
        public void AddControl(IContainer name, IContainer control)
        {
            _controls.Add(name, control);
            control.Parent = this;
        }
        
        public IEnumerable<KeyValuePair<IContainer, Rectangle>> ChildrenWithLayouts(Rectangle layout)
        {
            var i = 0;
            foreach (var kv in _controls)
            {
                yield return
                    new KeyValuePair<IContainer, Rectangle>(
                        kv.Value, 
                        new Rectangle(
                            layout.X + Math.Min(layout.Width / 2, LabelMaxWidth), 
                            layout.Y + i * 20, 
                            layout.Width - Math.Min(layout.Width / 2, LabelMaxWidth), 
                            20));
                i++;
            }
        }
        
        public void Render(IRenderContext context, ISkinLayout skinLayout, ISkinDelegator skinDelegator, Rectangle layout)
        {
            skinDelegator.Render(context, layout, this);
            foreach (var kv in LabelsWithLayouts(layout))
            {
                kv.Key.Render(context, skinLayout, skinDelegator, kv.Value);
            }

            foreach (var kv in ChildrenWithLayouts(layout))
            {
                kv.Key.Render(context, skinLayout, skinDelegator, kv.Value);
            }
        }
        
        public IEnumerable<KeyValuePair<IContainer, Rectangle>> LabelsWithLayouts(Rectangle layout)
        {
            var i = 0;
            foreach (var kv in _controls)
            {
                yield return
                    new KeyValuePair<IContainer, Rectangle>(
                        kv.Key, 
                        new Rectangle(layout.X, layout.Y + i * 20, Math.Min(layout.Width / 2, LabelMaxWidth), 20));
                i++;
            }
        }
        
        public void Update(ISkinLayout skin, Rectangle layout, GameTime gameTime, ref bool stealFocus)
        {
        }
        
        public bool HandleEvent(ISkinLayout skin, Rectangle layout, IGameContext context, Event @event)
        {
            var mousePressEvent = @event as MousePressEvent;

            if (mousePressEvent != null && mousePressEvent.Button == MouseButton.Left)
            {
                this.Focus();
            }

            foreach (var kv in LabelsWithLayouts(layout))
            {
                if (kv.Key.HandleEvent(skin, kv.Value, context, @event))
                {
                    return true;
                }
            }

            foreach (var kv in ChildrenWithLayouts(layout))
            {
                if (kv.Key.HandleEvent(skin, kv.Value, context, @event))
                {
                    return true;
                }
            }

            return false;
        }
    }
}