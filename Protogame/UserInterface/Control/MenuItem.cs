using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Protogame
{
    public class MenuItem : IContainer
    {
        protected List<MenuItem> Items = new List<MenuItem>();

        public MenuItem()
        {
            Active = false;

            // Give menu items a higher visibility over other things.
            Order = 10;
        }
        
        public event EventHandler Click;
        
        public bool Active { get; set; }
        
        public IContainer[] Children => Items.Cast<IContainer>().ToArray();
        
        public bool Focused { get; set; }
        
        public int HoverCountdown { get; set; }
        
        public bool Hovered { get; set; }
        
        public int Order { get; set; }
        
        public IContainer Parent { get; set; }

        public object Userdata { get; set; }
        
        public string Text { get; set; }
        
        public int TextWidth { get; set; }
        
        public void AddChild(MenuItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (item.Parent != null)
            {
                throw new InvalidOperationException();
            }

            Items.Add(item);
            item.Parent = this;
        }

        public bool IsRenderingMenuList { get; set; }
        
        public virtual void Render(IRenderContext context, ISkinLayout skinLayout, ISkinDelegator skinDelegator, Rectangle layout)
        {
            TextWidth = (int)Math.Ceiling(skinDelegator.MeasureText(context, Text, this).X);
            skinDelegator.Render(context, layout, this);

            var childrenLayout = GetMenuListLayout(skinLayout, layout);
            if (Active && childrenLayout != null)
            {
                IsRenderingMenuList = true;
                skinDelegator.Render(context, childrenLayout.Value, this);
                foreach (var kv in GetMenuChildren(skinLayout, layout))
                {
                    kv.Key.Render(context, skinLayout, skinDelegator, kv.Value);
                }
            }
        }
        
        public IEnumerable<Rectangle> GetActiveChildrenLayouts(ISkinLayout skin, Rectangle layout)
        {
            yield return layout;
            if (!Active)
            {
                yield break;
            }

            var childrenLayout = GetMenuListLayout(skin, layout);
            if (childrenLayout == null)
            {
                yield break;
            }

            yield return childrenLayout.Value;
            foreach (var kv in GetMenuChildren(skin, layout))
            {
                foreach (var childLayout in kv.Key.GetActiveChildrenLayouts(skin, kv.Value))
                {
                    yield return childLayout;
                }
            }
        }
        
        public IEnumerable<KeyValuePair<MenuItem, Rectangle>> GetMenuChildren(ISkinLayout skin, Rectangle layout)
        {
            var childLayout = GetMenuListLayout(skin, layout);
            if (childLayout == null)
            {
                yield break;
            }

            var accumulated = 0;
            foreach (var child in Items)
            {
                yield return
                    new KeyValuePair<MenuItem, Rectangle>(
                        child, 
                        new Rectangle(
                            childLayout.Value.X, 
                            childLayout.Value.Y + accumulated, 
                            childLayout.Value.Width, 
                            skin.MenuItemHeight));
                accumulated += skin.MenuItemHeight;
            }
        }
        
        public Rectangle? GetMenuListLayout(ISkinLayout skin, Rectangle layout)
        {
            // The location of the child items depends on whether we're owned
            // by a main menu or not.
            if (Items.Count == 0)
            {
                return null;
            }

            var maxWidth = Items.Max(x => x.TextWidth) + skin.AdditionalMenuItemWidth;
            var maxHeight = Items.Count * skin.MenuItemHeight;
            if (Parent is MainMenu)
            {
                return new Rectangle(layout.X, layout.Y + layout.Height, maxWidth, maxHeight);
            }

            return new Rectangle(layout.X + layout.Width, layout.Y, maxWidth, maxHeight);
        }
        
        public virtual void Update(ISkinLayout skin, Rectangle layout, GameTime gameTime, ref bool stealFocus)
        {
            var mouse = Mouse.GetState();
            var leftPressed = mouse.LeftChanged(this) == ButtonState.Pressed;

            if (layout.Contains(mouse.X, mouse.Y))
            {
                Hovered = true;
                HoverCountdown = 5;
                if (leftPressed)
                {
                    Click?.Invoke(this, new EventArgs());

                    Active = true;
                }
            }

            var deactivate = true;
            foreach (var activeLayout in GetActiveChildrenLayouts(skin, layout))
            {
                if (activeLayout.Contains(mouse.X, mouse.Y))
                {
                    deactivate = false;
                    break;
                }
            }

            Hovered = !deactivate;
            if (leftPressed)
            {
                Active = !deactivate;
            }

            if (HoverCountdown == 0)
            {
                Hovered = false;
            }

            if (!(Parent is MainMenu))
            {
                Active = Hovered;
            }

            if (Active)
            {
                foreach (var kv in GetMenuChildren(skin, layout))
                {
                    kv.Key.Update(skin, kv.Value, gameTime, ref stealFocus);
                }

                this.Focus();
            }

            // If the menu item is active, we steal focus from any further updating by our parent.
            if (Active)
            {
                stealFocus = true;
            }
        }
        
        public bool HandleEvent(ISkinLayout skin, Rectangle layout, IGameContext context, Event @event)
        {
            return Active;
        }
    }
}