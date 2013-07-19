using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Protogame
{
    public class TreeView : IContainer
    {
        private List<TreeItem> m_Items = new List<TreeItem>();
        private TreeItem p_SelectedItem;

        public IContainer[] Children { get { return this.m_Items.Cast<IContainer>().ToArray(); } }
        public IContainer Parent { get; set; }
        public int Order { get; set; }
        public bool Focused { get; set; }

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
                    this.SelectedItemChanged(this, new SelectedItemChangedEventArgs(value));
                this.Focus();
            }
        }

        public event SelectedItemChangedEventHandler SelectedItemChanged;

        public void AddChild(TreeItem item)
        {
            if (item == null)
                throw new ArgumentNullException("item");
            if (item.Parent != null)
                throw new InvalidOperationException();
            this.m_Items.Add(item);
            item.Parent = this;
        }

        public void RemoveChild(TreeItem item)
        {
            this.m_Items.Remove(item);
            item.Parent = null;
        }

        public void RemoveAllChildren()
        {
            foreach (var item in this.m_Items)
                item.Parent = null;
            this.m_Items.Clear();
        }

        public class TreeEntry
        {
            public TreeItem Item;
            public Rectangle? Layout;
            public string Name;
            public string FullName;
            public List<TreeEntry> Children;
            public int SegmentCount;
        }

        private TreeEntry FindParentForItem(TreeEntry current, TreeItem item)
        {
            var segments = item.Text == null ? -1 : item.Text.Split('.').Length;
            if (current.SegmentCount == segments - 1 &&
                item.Text.StartsWith(current.FullName))
                return current;
            foreach (var child in current.Children)
            {
                var result = this.FindParentForItem(child, item);
                if (result != null)
                    return result;
            }
            return null;
        }

        private void BackfillParentsForItem(TreeEntry root, TreeItem item)
        {
            var segments = item.Text.Split('.').Reverse()
                .Where((x, id) => id >= 1).Reverse().ToArray();
            var current = root;
            var i = 1;
            var built = "";
            foreach (var segment in segments)
            {
                built += "." + segment;
                built = built.Trim('.');
                var next = current.Children.FirstOrDefault(x => x.Name == segment);
                if (next == null)
                {
                    var newItem = new TreeItem
                    {
                        Text = built,
                        Parent = this
                    };
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
                    current = next;
                i++;
            }
        }

        public TreeEntry BuildEntryGraph(Rectangle layout)
        {
            var root = new TreeEntry
            {
                Item = null,
                Layout = null,
                Name = null,
                FullName = "",
                Children = new List<TreeEntry>(),
                SegmentCount = 0
            };
            foreach (var item in this.m_Items.OrderBy(x => x.Text).ToArray())
            {
                var parent = this.FindParentForItem(root, item);
                if (parent == null)
                    this.BackfillParentsForItem(root, item);
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

        public List<TreeEntry> NormalizeTree(TreeEntry tree, bool exclude)
        {
            var list = new List<TreeEntry>();
            if (!exclude)
                list.Add(tree);
            foreach (var child in tree.Children)
                list.AddRange(this.NormalizeTree(child, false));
            return list;
        }

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

        public void Update(ISkin skin, Rectangle layout, GameTime gameTime, ref bool stealFocus)
        {
            foreach (var kv in this.GetChildrenWithLayouts(skin, layout))
                kv.Item.Update(skin, kv.Layout.Value, gameTime, ref stealFocus);

            var keyboard = Keyboard.GetState();
            var upPressed = keyboard.IsKeyPressed(Keys.Up);
            var downPressed = keyboard.IsKeyPressed(Keys.Down);
            if (this.SelectedItem != null && (upPressed || downPressed))
            {
                var tree = this.BuildEntryGraph(layout);
                var list = this.NormalizeTree(tree, true);
                var current = list.IndexOf(list.First(x => this.SelectedItem == x.Item));
                if (upPressed)
                    current -= 1;
                else if (downPressed)
                    current += 1;
                if (current >= 0 && current < list.Count)
                    this.SelectedItem = list[current].Item;
            }
        }

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
    }
}

