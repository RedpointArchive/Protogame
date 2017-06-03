using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Protogame
{
    public class ScrollableContainer : IContainer
    {
        private IContainer _child;

        private RenderTarget2D _renderTarget;

        private bool _isHorizontalScrolling;

        private int _horizontalScrollOffset;

        private int _horizontalScrollStart;

        private bool _isVerticalScrolling;

        private int _verticalScrollOffset;

        private int _verticalScrollStart;

        public IContainer[] Children => new[] { _child };

        public bool Focused { get; set; }

        public int Order { get; set; }

        public IContainer Parent { get; set; }

        public object Userdata { get; set; }
        
        public float ScrollX { get; set; }
        
        public float ScrollY { get; set; }

        public RenderTarget2D ChildContent => _renderTarget;

        public bool NeedsVerticalScrollbar { get; private set; }

        public bool NeedsHorizontalScrollbar { get; private set; }

        public int ChildHeight { get; private set; }

        public int ChildWidth { get; private set; }

        public virtual void Render(IRenderContext context, ISkinLayout skinLayout, ISkinDelegator skinDelegator, Rectangle layout)
        {
            var constrainedLayoutWidth = layout.Width - skinLayout.VerticalScrollBarWidth;
            var constrainedLayoutHeight = layout.Height - skinLayout.HorizontalScrollBarHeight;

            var realLayoutWidth = layout.Width;
            var realLayoutHeight = layout.Height;

            NeedsVerticalScrollbar = false;
            NeedsHorizontalScrollbar = false;

            int childWidth, childHeight;
            if (!(_child is IHasDesiredSize))
            {
                childWidth = layout.Width;
                childHeight = layout.Height;
            }
            else
            {
                var hasDesiredSize = (IHasDesiredSize) _child;
                childWidth = hasDesiredSize.GetDesiredWidth(skinLayout) ?? layout.Width;
                childHeight = hasDesiredSize.GetDesiredHeight(skinLayout) ?? layout.Height;
                if (childHeight > layout.Height)
                {
                    NeedsVerticalScrollbar = true;
                    realLayoutWidth = constrainedLayoutWidth;

                    // Introducing a vertical scrollbar modifies the width, so update that.
                    childWidth = hasDesiredSize.GetDesiredWidth(skinLayout) ?? constrainedLayoutWidth;

                    if (childWidth > constrainedLayoutWidth)
                    {
                        NeedsHorizontalScrollbar = true;
                        realLayoutHeight = constrainedLayoutHeight;
                    }
                }
                else if (childWidth > layout.Width)
                {
                    NeedsHorizontalScrollbar = true;
                    realLayoutHeight = constrainedLayoutHeight;

                    // Introducing a horizontal scrollbar modifies the height, so update that.
                    childHeight = hasDesiredSize.GetDesiredHeight(skinLayout) ?? constrainedLayoutHeight;

                    if (childHeight > constrainedLayoutHeight)
                    {
                        NeedsVerticalScrollbar = true;
                        realLayoutWidth = constrainedLayoutWidth;
                    }
                }

                if (childWidth < realLayoutWidth)
                {
                    childWidth = realLayoutWidth;
                }
                if (childHeight < realLayoutHeight)
                {
                    childHeight = realLayoutHeight;
                }
            }

            if (_renderTarget == null || _renderTarget.Width != realLayoutWidth ||
                _renderTarget.Height != realLayoutHeight)
            {
                _renderTarget?.Dispose();

                _renderTarget = new RenderTarget2D(
                    context.GraphicsDevice,
                    Math.Max(1, realLayoutWidth),
                    Math.Max(1, realLayoutHeight));
            }

            ChildWidth = childWidth;
            ChildHeight = childHeight;
            
            context.SpriteBatch.End();
            context.PushRenderTarget(_renderTarget);
            context.GraphicsDevice.Clear(Color.Transparent);
            context.SpriteBatch.Begin();

            try
            {
                var scrollableChild = _child as IScrollableAwareChild;

                if (scrollableChild != null)
                {
                    scrollableChild.Render(
                        context,
                        skinLayout,
                        skinDelegator,
                        new Rectangle(
                            -(int)(ScrollX * (System.Math.Max(childWidth, realLayoutWidth) - realLayoutWidth)),
                            -(int)(ScrollY * (System.Math.Max(childHeight, realLayoutHeight) - realLayoutHeight)),
                            childWidth,
                            childHeight),
                        new Rectangle(
                            0,
                            0,
                            realLayoutWidth,
                            realLayoutHeight));
                }
                else
                {
                    _child?.Render(
                        context,
                        skinLayout,
                        skinDelegator,
                        new Rectangle(
                            -(int)(ScrollX * (System.Math.Max(childWidth, realLayoutWidth) - realLayoutWidth)),
                            -(int)(ScrollY * (System.Math.Max(childHeight, realLayoutHeight) - realLayoutHeight)),
                            childWidth,
                            childHeight));
                }
            }
            finally
            {
                context.SpriteBatch.End();
                context.PopRenderTarget();
                context.SpriteBatch.Begin();
            }

            skinDelegator.Render(context, layout, this);
        }

        public void SetChild(IContainer child)
        {
            if (child == null)
            {
                throw new ArgumentNullException(nameof(child));
            }

            if (child.Parent != null)
            {
                throw new InvalidOperationException();
            }

            _child = child;
            _child.Parent = this;
        }

        public virtual void Update(ISkinLayout skinLayout, Rectangle layout, GameTime gameTime, ref bool stealFocus)
        {
            _child?.Update(skinLayout, layout, gameTime, ref stealFocus);
        }

        private bool AnyChildFocused(IContainer container)
        {
            foreach (var child in container.Children)
            {
                if (child?.Focused ?? false)
                {
                    return true;
                }

                if (child != null)
                {
                    if (!(child is ScrollableContainer))
                    {
                        if (AnyChildFocused(child))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public bool HandleEvent(ISkinLayout skinLayout, Rectangle layout, IGameContext context, Event @event)
        {
            if (_renderTarget == null)
            {
                return false;
            }

            var layoutWidth = layout.Width - (NeedsVerticalScrollbar ? skinLayout.VerticalScrollBarWidth : 0);
            var layoutHeight = layout.Height - (NeedsHorizontalScrollbar ? skinLayout.HorizontalScrollBarHeight : 0);
            
            var mouseScrollEvent = @event as MouseScrollEvent;

            if (mouseScrollEvent != null)
            {
                if (this.Focused || AnyChildFocused(this))
                {
                    var scrollAmount = (1f / ChildHeight) * 40f;
                    ScrollY += (mouseScrollEvent.ScrollDelta / 120f) * scrollAmount;
                    if (ScrollY < 0)
                    {
                        ScrollY = 0;
                    }
                    if (ScrollY > 1)
                    {
                        ScrollY = 1;
                    }
                }
            }

            if (_isVerticalScrolling)
            {
                var mouseMoveEvent = @event as MouseMoveEvent;
                var mouseReleaseEvent = @event as MouseReleaseEvent;

                if (mouseMoveEvent != null)
                {
                    var newVerticalScrollbarPosition = mouseMoveEvent.Y - layout.Y - _verticalScrollOffset;
                    newVerticalScrollbarPosition = MathHelper.Clamp(
                        newVerticalScrollbarPosition,
                        0,
                        layoutHeight - (int)(layoutHeight / (float)ChildHeight * layoutHeight));
                    ScrollY = newVerticalScrollbarPosition /
                        (layoutHeight - layoutHeight / (float)ChildHeight * layoutHeight);
                    return true;
                }

                if (mouseReleaseEvent?.Button == MouseButton.Left)
                {
                    _isVerticalScrolling = false;
                }
            }
            else if (_isHorizontalScrolling)
            {
                var mouseMoveEvent = @event as MouseMoveEvent;
                var mouseReleaseEvent = @event as MouseReleaseEvent;

                if (mouseMoveEvent != null)
                {
                    var newHorizontalScrollbarPosition = mouseMoveEvent.X - layout.X - _horizontalScrollOffset;
                    newHorizontalScrollbarPosition = MathHelper.Clamp(
                        newHorizontalScrollbarPosition,
                        0,
                        layoutWidth - (int)(layoutWidth / (float)ChildWidth * layoutWidth));
                    ScrollX = newHorizontalScrollbarPosition /
                        (layoutWidth - layoutWidth / (float)ChildWidth * layoutWidth);
                    return true;
                }

                if (mouseReleaseEvent?.Button == MouseButton.Left)
                {
                    _isHorizontalScrolling = false;
                }
            }
            else if (!_isHorizontalScrolling && !_isVerticalScrolling)
            {
                var mousePressEvent = @event as MousePressEvent;

                var horizontalScrollBarRectangle = new Rectangle(
                    (int)(layout.X + ScrollX * (layoutWidth - layoutWidth / (float)ChildWidth * layoutWidth)),
                    layout.Y + layout.Height - skinLayout.HorizontalScrollBarHeight,
                    Math.Max((int)(layoutWidth / (float)ChildWidth * layoutWidth), 16),
                    skinLayout.HorizontalScrollBarHeight);
                var verticalScrollBarRectangle = new Rectangle(
                    layout.X + layout.Width - skinLayout.VerticalScrollBarWidth,
                    (int)(layout.Y + ScrollY * (layoutHeight - layoutHeight / (float)ChildHeight * layoutHeight)),
                    skinLayout.VerticalScrollBarWidth,
                    Math.Max((int)(layoutHeight / (float)ChildHeight * layoutHeight), 16));
               
                if (mousePressEvent != null && mousePressEvent.Button == MouseButton.Left)
                {
                    if (horizontalScrollBarRectangle.Contains(mousePressEvent.Position))
                    {
                        if (ChildWidth > layout.Width)
                        {
                            _isHorizontalScrolling = true;
                            _horizontalScrollOffset = mousePressEvent.X - horizontalScrollBarRectangle.X;
                            _horizontalScrollStart = mousePressEvent.X;
                        }

                        return true;
                    }

                    if (verticalScrollBarRectangle.Contains(mousePressEvent.Position))
                    {
                        if (ChildHeight > layout.Height)
                        {
                            _isVerticalScrolling = true;
                            _verticalScrollOffset = mousePressEvent.Y - verticalScrollBarRectangle.Y;
                            _verticalScrollStart = mousePressEvent.Y;
                        }

                        return true;
                    }
                }

                if (_child == null)
                {
                    return false;
                }

                var mouseEvent = @event as MouseEvent;

                var originalState = default(MouseState);
                int scrollXPixels = 0, scrollYPixels = 0;
                if (mouseEvent != null)
                {
                    scrollXPixels = (int)(ScrollX * (Math.Max(ChildWidth, layoutWidth) - layoutWidth));
                    scrollYPixels = (int)(ScrollY * (Math.Max(ChildHeight, layoutHeight) - layoutHeight));

                    mouseEvent.X += scrollXPixels;
                    mouseEvent.Y += scrollYPixels;

                    var mouseMoveEvent = @event as MouseMoveEvent;

                    if (mouseMoveEvent != null)
                    {
                        mouseMoveEvent.LastX += scrollXPixels;
                        mouseMoveEvent.LastY += scrollYPixels;
                        mouseMoveEvent.X += scrollXPixels;
                        mouseMoveEvent.Y += scrollYPixels;
                    }
                }

                _child.HandleEvent(skinLayout, new Rectangle(
                    layout.X,
                    layout.Y,
                    layout.Width - (NeedsVerticalScrollbar ? skinLayout.VerticalScrollBarWidth : 0),
                    layout.Height - (NeedsHorizontalScrollbar ? skinLayout.HorizontalScrollBarHeight : 0)), context, @event);

                // Restore event state.
                if (mouseEvent != null)
                {
                    mouseEvent.X -= scrollXPixels;
                    mouseEvent.Y -= scrollYPixels;

                    var mouseMoveEvent = @event as MouseMoveEvent;

                    if (mouseMoveEvent != null)
                    {
                        mouseMoveEvent.LastX -= scrollXPixels;
                        mouseMoveEvent.LastY -= scrollYPixels;
                        mouseMoveEvent.X -= scrollXPixels;
                        mouseMoveEvent.Y -= scrollYPixels;
                    }
                }
            }

            return false;
        }
    }
}

