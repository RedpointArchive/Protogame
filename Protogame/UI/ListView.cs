using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Protogame
{
    public class ListView : IContainer
    {
        private List<ListItem> m_Items = new List<ListItem>();
        private ListItem p_SelectedItem;

        public IContainer[] Children
        {
            get
            {
                return this.m_Items.Cast<IContainer>().ToArray();
            }
        }
        public IContainer Parent
        {
            get;
            set;
        }
        public int Order
        {
            get;
            set;
        }
        public bool Focused
        {
            get;
            set;
        }

        public ListItem SelectedItem
        {
            get
            {
                return this.p_SelectedItem;
            }
            set
            {
                this.p_SelectedItem = value;
                if (this.SelectedItemChanged != null)
                    this.SelectedItemChanged(this, new SelectedItemChangedEventArgs<ListItem>(value));
                this.Focus();
            }
        }

        public event SelectedItemChangedEventHandler<ListItem> SelectedItemChanged;

        public void AddChild(ListItem item)
        {
            if (item == null)
                throw new ArgumentNullException("item");
            if (item.Parent != null)
                throw new InvalidOperationException();
            this.m_Items.Add(item);
            item.Parent = this;
        }

        public void RemoveChild(ListItem item)
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

        public class ListEntry
        {
            public ListItem Item;
            public Rectangle? Layout;
        }
        
        public IEnumerable<ListEntry> GetChildrenWithLayouts(ISkin skin, Rectangle layout)
        {
            var list = this.m_Items.Select(x => new ListEntry { Item = x }).ToList();
            for (var i = 0; i < list.Count; i++)
            {
                list[i].Layout = new Rectangle(
                    layout.X,
                    layout.Y + i * skin.HeightForTreeItem,
                    layout.Width,
                    skin.HeightForTreeItem);
                yield return list[i];
            }
        }

        public void Update(ISkin skin, Rectangle layout, GameTime gameTime, ref bool stealFocus)
        {
            foreach (var kv in this.GetChildrenWithLayouts(skin, layout))
                kv.Item.Update(skin, kv.Layout.Value, gameTime, ref stealFocus);

            var keyboard = Keyboard.GetState();
            var upPressed = keyboard.IsKeyPressed(this, Keys.Up);
            var downPressed = keyboard.IsKeyPressed(this, Keys.Down);
            if (this.SelectedItem != null && (upPressed || downPressed))
            {
                var list = this.m_Items.Select(x => new ListEntry
                {
                    Item = x
                }).ToList();
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
            skin.DrawListView(context, layout, this);
            foreach (var kv in this.GetChildrenWithLayouts(skin, layout))
            {
                kv.Item.Draw(context, skin, kv.Layout.Value);
            }
        }
    }
}
