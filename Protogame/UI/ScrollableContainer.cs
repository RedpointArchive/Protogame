using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Protogame
{
    public class ScrollableContainer : IContainer
    {
        private IContainer m_Child;

        private RenderTarget2D m_RenderTarget;

        public IContainer[] Children
        {
            get
            {
                return new[] { this.m_Child };
            }
        }

        public bool Focused { get; set; }

        public int Order { get; set; }

        public IContainer Parent { get; set; }

        /// <summary>
        /// The scroll percentage between 0f and 1f.
        /// </summary>
        /// <value>The scroll x.</value>
        public float ScrollX { get; set; }

        /// <summary>
        /// The scroll percentage between 0f and 1f.
        /// </summary>
        /// <value>The scroll x.</value>
        public float ScrollY { get; set; }

        public virtual void Draw(IRenderContext context, ISkin skin, Rectangle layout)
        {
            var layoutWidth = layout.Width - skin.HorizontalScrollBarHeight;
            var layoutHeight = layout.Height - skin.VerticalScrollBarWidth;

            int childWidth, childHeight;
            if (this.m_Child == null || !(this.m_Child is IHasDesiredSize))
            {
                childWidth = layoutWidth;
                childHeight = layoutHeight;
            }
            else
            {
                var hasDesiredSize = this.m_Child as IHasDesiredSize;
                childWidth = hasDesiredSize.DesiredWidth ?? layoutWidth;
                childHeight = hasDesiredSize.DesiredHeight ?? layoutHeight;
            }

            if (this.m_RenderTarget == null || this.m_RenderTarget.Width != childWidth ||
                this.m_RenderTarget.Height != childHeight)
            {
                if (this.m_RenderTarget != null)
                {
                    this.m_RenderTarget.Dispose();
                }

                this.m_RenderTarget = new RenderTarget2D(
                    context.GraphicsDevice,
                    childWidth,
                    childHeight);
            }

            skin.BeforeRenderTargetChange(context);
            context.PushRenderTarget(this.m_RenderTarget);
            context.GraphicsDevice.Clear(Color.Transparent);
            skin.AfterRenderTargetChange(context);
            try
            {
                if (this.m_Child != null)
                {
                    this.m_Child.Draw(
                        context,
                        skin,
                        new Rectangle(0, 0, childWidth, childHeight));
                }
            }
            finally
            {
                skin.BeforeRenderTargetChange(context);
                context.PopRenderTarget();
                skin.AfterRenderTargetChange(context);
            }

            skin.DrawScrollableContainer(context, layout, this, this.m_RenderTarget);
        }

        public void SetChild(IContainer child)
        {
            if (child == null)
            {
                throw new ArgumentNullException("child");
            }

            if (child.Parent != null)
            {
                throw new InvalidOperationException();
            }

            this.m_Child = child;
            this.m_Child.Parent = this;
        }

        public virtual void Update(ISkin skin, Rectangle layout, GameTime gameTime, ref bool stealFocus)
        {
            if (this.m_Child != null)
            {
                this.m_Child.Update(skin, layout, gameTime, ref stealFocus);
            }
        }

        private bool m_IsHorizontalScrolling;

        private int m_HorizontalScrollOffset;

        private int m_HorizontalScrollStart;

        private bool m_IsVerticalScrolling;

        private int m_VerticalScrollOffset;

        private int m_VerticalScrollStart;

        public bool HandleEvent(ISkin skin, Rectangle layout, IGameContext context, Event @event)
        {
            if (this.m_RenderTarget == null)
            {
                return false;
            }

            var layoutWidth = layout.Width - skin.HorizontalScrollBarHeight;
            var layoutHeight = layout.Height - skin.VerticalScrollBarWidth;

            if (this.m_IsVerticalScrolling)
            {
                var mouseMoveEvent = @event as MouseMoveEvent;
                var mouseReleaseEvent = @event as MouseReleaseEvent;

                if (mouseMoveEvent != null)
                {
                    var newVerticalScrollbarPosition = mouseMoveEvent.Y - layout.Y - this.m_VerticalScrollOffset;
                    newVerticalScrollbarPosition = MathHelper.Clamp(
                        newVerticalScrollbarPosition,
                        0,
                        layoutHeight - (int)((layoutHeight / (float)this.m_RenderTarget.Height) * layoutHeight));
                    this.ScrollY = newVerticalScrollbarPosition /
                        (layoutHeight - ((layoutHeight / (float)this.m_RenderTarget.Height) * layoutHeight));
                    return true;
                }
                else if (mouseReleaseEvent != null)
                {
                    if (mouseReleaseEvent.Button == MouseButton.Left)
                    {
                        this.m_IsVerticalScrolling = false;
                    }
                }
            }
            else if (this.m_IsHorizontalScrolling)
            {
                var mouseMoveEvent = @event as MouseMoveEvent;
                var mouseReleaseEvent = @event as MouseReleaseEvent;

                if (mouseMoveEvent != null)
                {
                    var newHorizontalScrollbarPosition = mouseMoveEvent.X - layout.X - this.m_HorizontalScrollOffset;
                    newHorizontalScrollbarPosition = MathHelper.Clamp(
                        newHorizontalScrollbarPosition,
                        0,
                        layoutWidth - (int)((layoutWidth / (float)this.m_RenderTarget.Width) * layoutWidth));
                    this.ScrollX = newHorizontalScrollbarPosition /
                        (layoutWidth - ((layoutWidth / (float)this.m_RenderTarget.Width) * layoutWidth));
                    return true;
                }
                else if (mouseReleaseEvent != null)
                {
                    if (mouseReleaseEvent.Button == MouseButton.Left)
                    {
                        this.m_IsHorizontalScrolling = false;
                    }
                }
            }
            else if (!this.m_IsHorizontalScrolling && !this.m_IsVerticalScrolling)
            {
                var mousePressEvent = @event as MousePressEvent;

                var horizontalScrollBarRectangle = new Rectangle(
                    (int)(layout.X + this.ScrollX * (layoutWidth - ((layoutWidth / (float)this.m_RenderTarget.Width) * layoutWidth))),
                    layout.Y + layout.Height - skin.HorizontalScrollBarHeight,
                    (int)((layoutWidth / (float)this.m_RenderTarget.Width) * layoutWidth),
                    skin.HorizontalScrollBarHeight);
                var verticalScrollBarRectangle = new Rectangle(
                    layout.X + layout.Width - skin.VerticalScrollBarWidth,
                    (int)(layout.Y + this.ScrollY * (layoutHeight - ((layoutHeight / (float)this.m_RenderTarget.Height) * layoutHeight))),
                    skin.VerticalScrollBarWidth,
                    (int)((layoutHeight / (float)this.m_RenderTarget.Height) * layoutHeight));
               
                if (mousePressEvent != null && mousePressEvent.Button == MouseButton.Left)
                {
                    if (horizontalScrollBarRectangle.Contains(mousePressEvent.MouseState.Position))
                    {
                        this.m_IsHorizontalScrolling = true;
                        this.m_HorizontalScrollOffset = mousePressEvent.MouseState.Position.X - horizontalScrollBarRectangle.X;
                        this.m_HorizontalScrollStart = mousePressEvent.MouseState.Position.X;
                        return true;
                    }
                    else if (verticalScrollBarRectangle.Contains(mousePressEvent.MouseState.Position))
                    {
                        this.m_IsVerticalScrolling = true;
                        this.m_VerticalScrollOffset = mousePressEvent.MouseState.Position.Y - verticalScrollBarRectangle.Y;
                        this.m_VerticalScrollStart = mousePressEvent.MouseState.Position.Y;
                        return true;
                    }
                }

                if (this.m_Child != null)
                {
                    return this.m_Child.HandleEvent(skin, layout, context, @event);
                }
            }

            return false;
        }
    }
}

