namespace Protogame
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;

    /// <summary>
    /// The tree view.
    /// </summary>
    public class TreeView : IContainer
    {
        /// <summary>
        /// The m_ items.
        /// </summary>
        private readonly List<TreeItem> m_Items = new List<TreeItem>();

        /// <summary>
        /// The p_ selected item.
        /// </summary>
        private TreeItem p_SelectedItem;

        /// <summary>
        /// The selected item changed.
        /// </summary>
        public event SelectedItemChangedEventHandler<TreeItem> SelectedItemChanged;

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
        public TreeItem SelectedItem
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
                    this.SelectedItemChanged(this, new SelectedItemChangedEventArgs<TreeItem>(value));
                }

                this.Focus();
            }
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
        public void AddChild(TreeItem item)
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
        /// The build entry graph.
        /// </summary>
        /// <param name="layout">
        /// The layout.
        /// </param>
        /// <returns>
        /// The <see cref="TreeEntry"/>.
        /// </returns>
        public TreeEntry BuildEntryGraph(Rectangle layout)
        {
            var root = new TreeEntry
            {
                Item = null, 
                Layout = null, 
                Name = null, 
                FullName = string.Empty, 
                Children = new List<TreeEntry>(), 
                SegmentCount = 0
            };
            foreach (var item in this.m_Items.OrderBy(x => x.Text).ToArray())
            {
                var parent = this.FindParentForItem(root, item);
                if (parent == null)
                {
                    this.BackfillParentsForItem(root, item);
                }

                parent = this.FindParentForItem(root, item);
                var entry = new TreeEntry
                {
                    Item = item, 
                    Layout = null, 
                    Name = item.Text.Split('.').Last(), 
                    FullName = (parent.FullName + "." + item.Text.Split('.').Last()).Trim('.'), 
                    Children = new List<TreeEntry>(), 
                    SegmentCount = parent.SegmentCount + 1
                };
                parent.Children.Add(entry);
            }

            return root;
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
            skin.DrawTreeView(context, layout, this);
            foreach (var kv in this.GetChildrenWithLayouts(skin, layout))
            {
                var old = kv.Item.Text;
                kv.Item.Text = kv.Name;
                kv.Item.Draw(context, skin, kv.Layout.Value);
                kv.Item.Text = old;
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
        public IEnumerable<TreeEntry> GetChildrenWithLayouts(ISkin skin, Rectangle layout)
        {
            var tree = this.BuildEntryGraph(layout);
            var list = this.NormalizeTree(tree, true);
            for (var i = 0; i < list.Count; i++)
            {
                list[i].Layout = new Rectangle(
                    layout.X + (list[i].SegmentCount - 1) * skin.HeightForTreeItem, 
                    layout.Y + i * skin.HeightForTreeItem, 
                    layout.Width - (list[i].SegmentCount - 1) * skin.HeightForTreeItem, 
                    skin.HeightForTreeItem);
                yield return list[i];
            }
        }

        /// <summary>
        /// The normalize tree.
        /// </summary>
        /// <param name="tree">
        /// The tree.
        /// </param>
        /// <param name="exclude">
        /// The exclude.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<TreeEntry> NormalizeTree(TreeEntry tree, bool exclude)
        {
            var list = new List<TreeEntry>();
            if (!exclude)
            {
                list.Add(tree);
            }

            foreach (var child in tree.Children)
            {
                list.AddRange(this.NormalizeTree(child, false));
            }

            return list;
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
        public void RemoveChild(TreeItem item)
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

            var keyboard = Keyboard.GetState();
            var upPressed = keyboard.IsKeyPressed(this, Keys.Up);
            var downPressed = keyboard.IsKeyPressed(this, Keys.Down);
            if (this.SelectedItem != null && (upPressed || downPressed))
            {
                var tree = this.BuildEntryGraph(layout);
                var list = this.NormalizeTree(tree, true);
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
                }
            }
        }

        /// <summary>
        /// The backfill parents for item.
        /// </summary>
        /// <param name="root">
        /// The root.
        /// </param>
        /// <param name="item">
        /// The item.
        /// </param>
        private void BackfillParentsForItem(TreeEntry root, TreeItem item)
        {
            var segments = item.Text.Split('.').Reverse().Where((x, id) => id >= 1).Reverse().ToArray();
            var current = root;
            var i = 1;
            var built = string.Empty;
            foreach (var segment in segments)
            {
                built += "." + segment;
                built = built.Trim('.');
                var next = current.Children.FirstOrDefault(x => x.Name == segment);
                if (next == null)
                {
                    var newItem = new TreeItem { Text = built, Parent = this };
                    this.m_Items.Add(newItem);
                    var created = new TreeEntry
                    {
                        Item = newItem, 
                        Layout = null, 
                        Name = segment, 
                        FullName = built, 
                        Children = new List<TreeEntry>(), 
                        SegmentCount = i
                    };
                    current.Children.Add(created);
                    current = created;
                }
                else
                {
                    current = next;
                }

                i++;
            }
        }

        /// <summary>
        /// The find parent for item.
        /// </summary>
        /// <param name="current">
        /// The current.
        /// </param>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <returns>
        /// The <see cref="TreeEntry"/>.
        /// </returns>
        private TreeEntry FindParentForItem(TreeEntry current, TreeItem item)
        {
            var segments = item.Text == null ? -1 : item.Text.Split('.').Length;
            if (current.SegmentCount == segments - 1 && item.Text.StartsWith(current.FullName))
            {
                return current;
            }

            foreach (var child in current.Children)
            {
                var result = this.FindParentForItem(child, item);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }

        /// <summary>
        /// The tree entry.
        /// </summary>
        public class TreeEntry
        {
            /// <summary>
            /// The children.
            /// </summary>
            public List<TreeEntry> Children;

            /// <summary>
            /// The full name.
            /// </summary>
            public string FullName;

            /// <summary>
            /// The item.
            /// </summary>
            public TreeItem Item;

            /// <summary>
            /// The layout.
            /// </summary>
            public Rectangle? Layout;

            /// <summary>
            /// The name.
            /// </summary>
            public string Name;

            /// <summary>
            /// The segment count.
            /// </summary>
            public int SegmentCount;
        }
    }
}