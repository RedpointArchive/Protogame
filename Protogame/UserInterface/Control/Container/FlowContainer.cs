using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace Protogame
{
    public abstract class FlowContainer : IContainer
    {
        private readonly List<IContainer> _children = new List<IContainer>();
        
        private readonly List<string> _sizes = new List<string>();
        
        public IContainer[] Children => _children.ToArray();
        
        public bool Focused { get; set; }
        
        public int Order { get; set; }
        
        public IContainer Parent { get; set; }

        public object Userdata { get; set; }
        
        public void AddChild(IContainer child, string size)
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
            _sizes.Add(size);
            child.Parent = this;
        }
        
        public IEnumerable<KeyValuePair<IContainer, Rectangle>> ChildrenWithLayouts(Rectangle layout)
        {
            var initialPass = new List<int?>();
            var finalPass = new List<int>();
            var variedCount = 0;
            foreach (var s in _sizes)
            {
                if (s.EndsWith("%", StringComparison.Ordinal))
                {
                    initialPass.Add(
                        (int)(GetMaximumContainerSize(layout) * (Convert.ToInt32(s.TrimEnd('%')) / 100f)));
                }
                else if (s == "*")
                {
                    variedCount += 1;
                    initialPass.Add(null);
                }
                else
                {
                    initialPass.Add(Convert.ToInt32(s));
                }
            }

            var total = initialPass.Where(x => x != null).Select(x => x.Value).Sum();
            var remaining = Math.Max(0, GetMaximumContainerSize(layout) - total);
            foreach (var i in initialPass)
            {
                if (i == null)
                {
                    finalPass.Add(remaining / variedCount);
                }
                else
                {
                    finalPass.Add(i.Value);
                }
            }

            var accumulated = 0;
            for (var i = 0; i < _children.Count; i++)
            {
                var childLayout = CreateChildLayout(layout, accumulated, finalPass[i]);
                yield return new KeyValuePair<IContainer, Rectangle>(_children[i], childLayout);
                accumulated += finalPass[i];
            }
        }
        
        public abstract void Render(IRenderContext context, ISkinLayout skinLayout, ISkinDelegator skinDelegator, Rectangle layout);
        
        public void RemoveChild(IContainer child)
        {
            _sizes.RemoveAt(_children.IndexOf(child));
            _children.Remove(child);
            child.Parent = null;
        }
        
        public void SetChildSize(IContainer child, string size)
        {
            var index = _children.IndexOf(child);
            _sizes.RemoveAt(index);
            _sizes.Insert(index, size);
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
        
        protected abstract Rectangle CreateChildLayout(Rectangle layout, int accumulated, int size);
        
        protected abstract int GetMaximumContainerSize(Rectangle layout);
    }
}