using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Protogame
{    
    public class ListView : IContainer, IHasDesiredSize
    {
        private readonly List<IContainer> _items = new List<IContainer>();
        
        private IContainer _selectedItem;
        
        public event SelectedItemChangedEventHandler<IContainer> SelectedItemChanged;
        
        public IContainer[] Children => _items.Cast<IContainer>().ToArray();
        
        public bool Focused { get; set; }
        
        public int Order { get; set; }
        
        public IContainer Parent { get; set; }

        public object Userdata { get; set; }
        
        public IContainer SelectedItem
        {
            get
            {
                return _selectedItem;
            }

            set
            {
                _selectedItem = value;
                SelectedItemChanged?.Invoke(this, new SelectedItemChangedEventArgs<IContainer>(value));

                this.Focus();
            }
        }

        public int? GetDesiredWidth(ISkinLayout skin)
        {
            return null;
        }

        public int? GetDesiredHeight(ISkinLayout skin)
        {
            var current = 0;
            foreach (var item in _items)
            {
                var desiredSize = item as IHasDesiredSize;
                if (desiredSize?.GetDesiredHeight(skin) != null)
                {
                    var desiredHeight = desiredSize.GetDesiredHeight(skin);
                    if (desiredHeight != null)
                    {
                        current += desiredHeight.Value;
                    }
                }
                else
                {
                    current += skin.HeightForTreeItem;
                }
            }
            return current;
        }
        
        public void AddChild(IContainer item)
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
        
        public void Render(IRenderContext context, ISkinLayout skinLayout, ISkinDelegator skinDelegator, Rectangle layout)
        {
            skinDelegator.Render(context, layout, this);
            foreach (var kv in GetChildrenWithLayouts(skinLayout, layout))
            {
                if (kv.Layout != null)
                {
                    kv.Item.Render(context, skinLayout, skinDelegator, kv.Layout.Value);
                }
            }
        }

        public IEnumerable<ListEntry> GetChildrenWithLayouts(ISkinLayout skinLayout, Rectangle layout)
        {
            var list = _items.Select(x => new ListEntry { Item = x }).ToList();
            var current = layout.Y;
            foreach (var item in list)
            {
                item.Layout = new Rectangle(
                    layout.X + skinLayout.ListHorizontalPadding, 
                    current + skinLayout.ListVerticalPadding, 
                    layout.Width - skinLayout.ListHorizontalPadding * 2,
                    skinLayout.HeightForTreeItem);
                yield return item;

                var desiredSize = item.Item as IHasDesiredSize;
                if (desiredSize?.GetDesiredHeight(skinLayout) != null)
                {
                    var desiredHeight = desiredSize.GetDesiredHeight(skinLayout);
                    if (desiredHeight != null)
                    {
                        current += desiredHeight.Value;
                    }
                }
                else
                {
                    current += skinLayout.HeightForTreeItem;
                }
            }
        }
        
        public void RemoveAllChildren()
        {
            foreach (var item in _items)
            {
                item.Parent = null;
            }

            _items.Clear();
        }
        
        public void RemoveChild(IContainer item)
        {
            _items.Remove(item);
            item.Parent = null;
        }
        
        public void Update(ISkinLayout skinLayout, Rectangle layout, GameTime gameTime, ref bool stealFocus)
        {
            foreach (var kv in GetChildrenWithLayouts(skinLayout, layout))
            {
                if (kv.Layout != null)
                {
                    kv.Item.Update(skinLayout, kv.Layout.Value, gameTime, ref stealFocus);
                }
            }
        }
        
        public bool HandleEvent(ISkinLayout skinLayout, Rectangle layout, IGameContext context, Event @event)
        {
            foreach (var kv in GetChildrenWithLayouts(skinLayout, layout))
            {
                if (kv.Layout != null && kv.Item.HandleEvent(skinLayout, kv.Layout.Value, context, @event))
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
                    var list = _items.Select(x => new ListEntry { Item = x }).ToList();
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
        
        public class ListEntry
        {
            public IContainer Item;
            
            public Rectangle? Layout;
        }
    }
}