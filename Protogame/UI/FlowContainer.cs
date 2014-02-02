namespace Protogame
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Xna.Framework;

    /// <summary>
    /// The flow container.
    /// </summary>
    public abstract class FlowContainer : IContainer
    {
        /// <summary>
        /// The m_ children.
        /// </summary>
        private readonly List<IContainer> m_Children = new List<IContainer>();

        /// <summary>
        /// The m_ sizes.
        /// </summary>
        private readonly List<string> m_Sizes = new List<string>();

        /// <summary>
        /// Gets the children.
        /// </summary>
        /// <value>
        /// The children.
        /// </value>
        public IContainer[] Children
        {
            get
            {
                return this.m_Children.ToArray();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether focused.
        /// </summary>
        /// <value>
        /// The focused.
        /// </value>
        public bool Focused { get; set; }

        /// <summary>
        /// Gets or sets the order.
        /// </summary>
        /// <value>
        /// The order.
        /// </value>
        public int Order { get; set; }

        /// <summary>
        /// Gets or sets the parent.
        /// </summary>
        /// <value>
        /// The parent.
        /// </value>
        public IContainer Parent { get; set; }

        /// <summary>
        /// The add child.
        /// </summary>
        /// <param name="child">
        /// The child.
        /// </param>
        /// <param name="size">
        /// The size.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// </exception>
        public void AddChild(IContainer child, string size)
        {
            if (child == null)
            {
                throw new ArgumentNullException("child");
            }

            if (child.Parent != null)
            {
                throw new InvalidOperationException();
            }

            this.m_Children.Add(child);
            this.m_Sizes.Add(size);
            child.Parent = this;
        }

        /// <summary>
        /// The children with layouts.
        /// </summary>
        /// <param name="layout">
        /// The layout.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        public IEnumerable<KeyValuePair<IContainer, Rectangle>> ChildrenWithLayouts(Rectangle layout)
        {
            var initialPass = new List<int?>();
            var finalPass = new List<int>();
            var variedCount = 0;
            foreach (var s in this.m_Sizes)
            {
                if (s.EndsWith("%", StringComparison.Ordinal))
                {
                    initialPass.Add(
                        (int)(this.GetMaximumContainerSize(layout) * (Convert.ToInt32(s.TrimEnd('%')) / 100f)));
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
            var remaining = Math.Max(0, this.GetMaximumContainerSize(layout) - total);
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
            for (var i = 0; i < this.m_Children.Count; i++)
            {
                var childLayout = this.CreateChildLayout(layout, accumulated, finalPass[i]);
                yield return new KeyValuePair<IContainer, Rectangle>(this.m_Children[i], childLayout);
                accumulated += finalPass[i];
            }
        }

        /// <summary>
        /// The draw.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="skin">
        /// The skin.
        /// </param>
        /// <param name="layout">
        /// The layout.
        /// </param>
        public abstract void Draw(IRenderContext context, ISkin skin, Rectangle layout);

        /// <summary>
        /// The remove child.
        /// </summary>
        /// <param name="child">
        /// The child.
        /// </param>
        public void RemoveChild(IContainer child)
        {
            this.m_Sizes.RemoveAt(this.m_Children.IndexOf(child));
            this.m_Children.Remove(child);
            child.Parent = null;
        }

        /// <summary>
        /// The set child size.
        /// </summary>
        /// <param name="child">
        /// The child.
        /// </param>
        /// <param name="size">
        /// The size.
        /// </param>
        public void SetChildSize(IContainer child, string size)
        {
            var index = this.m_Children.IndexOf(child);
            this.m_Sizes.RemoveAt(index);
            this.m_Sizes.Insert(index, size);
        }

        /// <summary>
        /// The update.
        /// </summary>
        /// <param name="skin">
        /// The skin.
        /// </param>
        /// <param name="layout">
        /// The layout.
        /// </param>
        /// <param name="gameTime">
        /// The game time.
        /// </param>
        /// <param name="stealFocus">
        /// The steal focus.
        /// </param>
        public void Update(ISkin skin, Rectangle layout, GameTime gameTime, ref bool stealFocus)
        {
            foreach (var kv in this.ChildrenWithLayouts(layout))
            {
                kv.Key.Update(skin, kv.Value, gameTime, ref stealFocus);
                if (stealFocus)
                {
                    break;
                }
            }
        }

        /// <summary>
        /// The create child layout.
        /// </summary>
        /// <param name="layout">
        /// The layout.
        /// </param>
        /// <param name="accumulated">
        /// The accumulated.
        /// </param>
        /// <param name="size">
        /// The size.
        /// </param>
        /// <returns>
        /// The <see cref="Rectangle"/>.
        /// </returns>
        protected abstract Rectangle CreateChildLayout(Rectangle layout, int accumulated, int size);

        /// <summary>
        /// The get maximum container size.
        /// </summary>
        /// <param name="layout">
        /// The layout.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        protected abstract int GetMaximumContainerSize(Rectangle layout);
    }
}