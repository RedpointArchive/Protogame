using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace Protogame
{
    public class RelativeContainer : IContainer
    {
        private readonly List<IContainer> _children = new List<IContainer>();

        private readonly List<Rectangle> _positions = new List<Rectangle>();

        public IContainer[] Children => _children.ToArray();

        public bool Focused { get; set; }

        public int Order { get; set; }

        public IContainer Parent { get; set; }

        public object Userdata { get; set; }

        public void AddChild(IContainer child, Rectangle rect)
        {
            if (child == null)
            {
                throw new ArgumentNullException(nameof(child));
            }

            if (child.Parent != null)
            {
                throw new InvalidOperationException();
            }

            _children.Add(child);
            _positions.Add(rect);
            child.Parent = this;
        }

        public IEnumerable<KeyValuePair<IContainer, Rectangle>> ChildrenWithLayouts(Rectangle layout)
        {
            for (var i = 0; i < _positions.Count; i++)
            {
                var position = _positions[i];
                var child = _children[i];

                var relativePosition = new Rectangle(layout.X + position.X, layout.Y + position.Y, position.Width,
                    position.Height);

                yield return new KeyValuePair<IContainer, Rectangle>(child, relativePosition);
            }
        }

        public void Render(IRenderContext context, ISkinLayout skinLayout, ISkinDelegator skinDelegator,
            Rectangle layout)
        {
            skinDelegator.Render(context, layout, this);
            foreach (var kv in this.ChildrenWithLayouts(layout).OrderByDescending(x => x.Key.Order))
            {
                kv.Key.Render(context, skinLayout, skinDelegator, kv.Value);
            }
        }

        public void RemoveChild(IContainer child)
        {
            _positions.RemoveAt(_children.IndexOf(child));
            _children.Remove(child);
            child.Parent = null;
        }

        public void SetChildRectangle(IContainer child, Rectangle rect)
        {
            var index = _children.IndexOf(child);
            _positions.RemoveAt(index);
            _positions.Insert(index, rect);
        }

        public void Update(ISkinLayout skinLayout, Rectangle layout, GameTime gameTime, ref bool stealFocus)
        {
            foreach (var kv in ChildrenWithLayouts(layout))
            {
                kv.Key.Update(skinLayout, kv.Value, gameTime, ref stealFocus);
                if (stealFocus)
                {
                    break;
                }
            }
        }

        public bool HandleEvent(ISkinLayout skinLayout, Rectangle layout, IGameContext context, Event @event)
        {
            foreach (var kv in ChildrenWithLayouts(layout))
            {
                if (kv.Key.HandleEvent(skinLayout, kv.Value, context, @event))
                {
                    return true;
                }
            }

            return false;
        }
    }
}