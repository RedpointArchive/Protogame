using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Protogame
{
    public class TreeView : IContainer
    {
        private readonly List<TreeItem> _items = new List<TreeItem>();
        
        private TreeItem p_SelectedItem;
        
        public event SelectedItemChangedEventHandler<TreeItem> SelectedItemChanged;
        
        public IContainer[] Children => _items.Cast<IContainer>().ToArray();

        public bool Focused { get; set; }
        
        public int Order { get; set; }
        
        public IContainer Parent { get; set; }

        public object Userdata { get; set; }
        
        public TreeItem SelectedItem
        {
            get
            {
                return p_SelectedItem;
            }

            set
            {
                p_SelectedItem = value;
                SelectedItemChanged?.Invoke(this, new SelectedItemChangedEventArgs<TreeItem>(value));

                this.Focus();
            }
        }
        
        public void AddChild(TreeItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (item.Parent != null)
            {
                throw new InvalidOperationException();
            }

            _items.Add(item);
            item.Parent = this;
        }
        
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
            foreach (var item in _items.OrderBy(x => x.Text).ToArray())
            {
                var parent = FindParentForItem(root, item);
                if (parent == null)
                {
                    BackfillParentsForItem(root, item);
                }

                parent = FindParentForItem(root, item);
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
        
        public void Render(IRenderContext context, ISkinLayout skinLayout, ISkinDelegator skinDelegator, Rectangle layout)
        {
            skinDelegator.Render(context, layout, this);
            foreach (var kv in GetChildrenWithLayouts(skinLayout, layout))
            {
                var old = kv.Item.Text;
                kv.Item.Text = kv.Name;
                if (kv.Layout != null)
                {
                    kv.Item.Render(context, skinLayout, skinDelegator, kv.Layout.Value);
                }
                kv.Item.Text = old;
            }
        }
        
        public IEnumerable<TreeEntry> GetChildrenWithLayouts(ISkinLayout skin, Rectangle layout)
        {
            var tree = BuildEntryGraph(layout);
            var list = NormalizeTree(tree, true);
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
        
        public List<TreeEntry> NormalizeTree(TreeEntry tree, bool exclude)
        {
            var list = new List<TreeEntry>();
            if (!exclude)
            {
                list.Add(tree);
            }

            foreach (var child in tree.Children)
            {
                list.AddRange(NormalizeTree(child, false));
            }

            return list;
        }
        
        public void RemoveAllChildren()
        {
            foreach (var item in _items)
            {
                item.Parent = null;
            }

            _items.Clear();
        }
        
        public void RemoveChild(TreeItem item)
        {
            _items.Remove(item);
            item.Parent = null;
        }
        
        public void Update(ISkinLayout skin, Rectangle layout, GameTime gameTime, ref bool stealFocus)
        {
            foreach (var kv in GetChildrenWithLayouts(skin, layout))
            {
                if (kv.Layout != null)
                {
                    kv.Item.Update(skin, kv.Layout.Value, gameTime, ref stealFocus);
                }
            }
        }
        
        public bool HandleEvent(ISkinLayout skin, Rectangle layout, IGameContext context, Event @event)
        {
            foreach (var kv in GetChildrenWithLayouts(skin, layout))
            {
                if (kv.Layout != null && kv.Item.HandleEvent(skin, kv.Layout.Value, context, @event))
                {
                    return true;
                }
            }

            var keyPressEvent = @event as KeyPressEvent;
            if (keyPressEvent != null)
            {
                var upPressed = keyPressEvent.Key == Keys.Up;
                var downPressed = keyPressEvent.Key == Keys.Down;
                if (SelectedItem != null && (upPressed || downPressed))
                {
                    var tree = BuildEntryGraph(layout);
                    var list = NormalizeTree(tree, true);
                    var current = list.IndexOf(list.First(x => SelectedItem == x.Item));
                    if (upPressed)
                    {
                        current -= 1;
                    }
                    else
                    {
                        current += 1;
                    }

                    if (current >= 0 && current < list.Count)
                    {
                        SelectedItem = list[current].Item;

                        return true;
                    }
                }
            }

            return false;
        }
        
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
                    _items.Add(newItem);
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
        
        private TreeEntry FindParentForItem(TreeEntry current, TreeItem item)
        {
            var segments = item.Text?.Split('.').Length ?? -1;
            if (item.Text != null && current.SegmentCount == segments - 1 && item.Text.StartsWith(current.FullName))
            {
                return current;
            }

            foreach (var child in current.Children)
            {
                var result = FindParentForItem(child, item);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }
        
        public class TreeEntry
        {
            public List<TreeEntry> Children;

            public string FullName;

            public TreeItem Item;

            public Rectangle? Layout;

            public string Name;

            public int SegmentCount;
        }
    }
}