namespace Protogame
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;

    /// <summary>
    /// The list view.
    /// </summary>
    public class ListView : IContainer, IHasDesiredSize
    {
        /// <summary>
        /// The m_ items.
        /// </summary>
        private readonly List<IContainer> m_Items = new List<IContainer>();

        /// <summary>
        /// The p_ selected item.
        /// </summary>
        private IContainer p_SelectedItem;

        /// <summary>
        /// The selected item changed.
        /// </summary>
        public event SelectedItemChangedEventHandler<IContainer> SelectedItemChanged;

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
                return this.m_Items.Cast<IContainer>().ToArray();
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
        /// Gets or sets the selected item.
        /// </summary>
        /// <value>
        /// The selected item.
        /// </value>
        public IContainer SelectedItem
        {
            get
            {
                return this.p_SelectedItem;
            }

            set
            {
                this.p_SelectedItem = value;
                if (this.SelectedItemChanged != null)
                {
                    this.SelectedItemChanged(this, new SelectedItemChangedEventArgs<IContainer>(value));
                }

                this.Focus();
            }
        }

        public int? GetDesiredWidth(ISkin skin)
        {
            return null;
        }

        public int? GetDesiredHeight(ISkin skin)
        {
            return this.Children.Length * skin.HeightForTreeItem;
        }

        /// <summary>
        /// The add child.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// </exception>
        public void AddChild(IContainer item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            if (item.Parent != null)
            {
                throw new InvalidOperationException();
            }

            this.m_Items.Add(item);
            item.Parent = this;
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
        public void Draw(IRenderContext context, ISkin skin, Rectangle layout)
        {
            skin.DrawListView(context, layout, this);
            foreach (var kv in this.GetChildrenWithLayouts(skin, layout))
            {
                kv.Item.Draw(context, skin, kv.Layout.Value);
            }
        }

        /// <summary>
        /// The get children with layouts.
        /// </summary>
        /// <param name="skin">
        /// The skin.
        /// </param>
        /// <param name="layout">
        /// The layout.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        public IEnumerable<ListEntry> GetChildrenWithLayouts(ISkin skin, Rectangle layout)
        {
            var list = this.m_Items.Select(x => new ListEntry { Item = x }).ToList();
            for (var i = 0; i < list.Count; i++)
            {
                list[i].Layout = new Rectangle(
                    layout.X + skin.ListHorizontalPadding, 
                    layout.Y + i * skin.HeightForTreeItem + skin.ListVerticalPadding, 
                    layout.Width - skin.ListHorizontalPadding * 2, 
                    skin.HeightForTreeItem);
                yield return list[i];
            }
        }

        /// <summary>
        /// The remove all children.
        /// </summary>
        public void RemoveAllChildren()
        {
            foreach (var item in this.m_Items)
            {
                item.Parent = null;
            }

            this.m_Items.Clear();
        }

        /// <summary>
        /// The remove child.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        public void RemoveChild(ListItem item)
        {
            this.m_Items.Remove(item);
            item.Parent = null;
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
            foreach (var kv in this.GetChildrenWithLayouts(skin, layout))
            {
                kv.Item.Update(skin, kv.Layout.Value, gameTime, ref stealFocus);
            }
        }

        /// <summary>
        /// Requests that the UI container handle the specified event or return false.
        /// </summary>
        /// <param name="skin">
        /// The UI skin.
        /// </param>
        /// <param name="layout">
        /// The layout for the element.
        /// </param>
        /// <param name="context">
        /// The current game context.
        /// </param>
        /// <param name="event">
        /// The event that was raised.
        /// </param>
        /// <returns>
        /// Whether or not this UI element handled the event.
        /// </returns>
        public bool HandleEvent(ISkin skin, Rectangle layout, IGameContext context, Event @event)
        {
            foreach (var kv in this.GetChildrenWithLayouts(skin, layout))
            {
                if (kv.Item.HandleEvent(skin, kv.Layout.Value, context, @event))
                {
                    return true;
                }
            }

            var keyPressEvent = @event as KeyPressEvent;
            if (keyPressEvent != null)
            {
                var upPressed = keyPressEvent.Key == Keys.Up;
                var downPressed = keyPressEvent.Key == Keys.Down;
                if (this.SelectedItem != null && (upPressed || downPressed))
                {
                    var list = this.m_Items.Select(x => new ListEntry { Item = x }).ToList();
                    var current = list.IndexOf(list.First(x => this.SelectedItem == x.Item));
                    if (upPressed)
                    {
                        current -= 1;
                    }
                    else if (downPressed)
                    {
                        current += 1;
                    }

                    if (current >= 0 && current < list.Count)
                    {
                        this.SelectedItem = list[current].Item;

                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// The list entry.
        /// </summary>
        public class ListEntry
        {
            /// <summary>
            /// The item.
            /// </summary>
            public IContainer Item;

            /// <summary>
            /// The layout.
            /// </summary>
            public Rectangle? Layout;
        }
    }
}