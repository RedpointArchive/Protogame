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

        public virtual void Render(IRenderContext context, ISkinLayout skinLayout, ISkinDelegator skinDelegator, Rectangle layout)
        {
            var layoutWidth = layout.Width - skinLayout.HorizontalScrollBarHeight;
            var layoutHeight = layout.Height - skinLayout.VerticalScrollBarWidth;

            int childWidth, childHeight;
            if (!(_child is IHasDesiredSize))
            {
                childWidth = layoutWidth;
                childHeight = layoutHeight;
            }
            else
            {
                var hasDesiredSize = (IHasDesiredSize) _child;
                childWidth = hasDesiredSize.GetDesiredWidth(skinLayout) ?? layoutWidth;
                childHeight = hasDesiredSize.GetDesiredHeight(skinLayout) ?? layoutHeight;
                if (childWidth < layoutWidth)
                {
                    childWidth = layoutWidth;
                }
                if (childHeight < layoutHeight)
                {
                    childHeight = layoutHeight;
                }
            }

            if (_renderTarget == null || _renderTarget.Width != childWidth ||
                _renderTarget.Height != childHeight)
            {
                _renderTarget?.Dispose();

                _renderTarget = new RenderTarget2D(
                    context.GraphicsDevice,
                    Math.Max(1, childWidth),
                    Math.Max(1, childHeight));
            }
            
            context.SpriteBatch.End();
            context.PushRenderTarget(_renderTarget);
            context.GraphicsDevice.Clear(Color.Transparent);
            context.SpriteBatch.Begin();

            try
            {
                _child?.Render(
                    context,
                    skinLayout,
                    skinDelegator,
                    new Rectangle(0, 0, childWidth, childHeight));
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

        public bool HandleEvent(ISkinLayout skinLayout, Rectangle layout, IGameContext context, Event @event)
        {
            if (_renderTarget == null)
            {
                return false;
            }

            var layoutWidth = layout.Width - skinLayout.HorizontalScrollBarHeight;
            var layoutHeight = layout.Height - skinLayout.VerticalScrollBarWidth;

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
                        layoutHeight - (int)(layoutHeight / (float)_renderTarget.Height * layoutHeight));
                    ScrollY = newVerticalScrollbarPosition /
                        (layoutHeight - layoutHeight / (float)_renderTarget.Height * layoutHeight);
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
                        layoutWidth - (int)(layoutWidth / (float)_renderTarget.Width * layoutWidth));
                    ScrollX = newHorizontalScrollbarPosition /
                        (layoutWidth - layoutWidth / (float)_renderTarget.Width * layoutWidth);
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
                    (int)(layout.X + ScrollX * (layoutWidth - layoutWidth / (float)_renderTarget.Width * layoutWidth)),
                    layout.Y + layout.Height - skinLayout.HorizontalScrollBarHeight,
                    (int)(layoutWidth / (float)_renderTarget.Width * layoutWidth),
                    skinLayout.HorizontalScrollBarHeight);
                var verticalScrollBarRectangle = new Rectangle(
                    layout.X + layout.Width - skinLayout.VerticalScrollBarWidth,
                    (int)(layout.Y + ScrollY * (layoutHeight - layoutHeight / (float)_renderTarget.Height * layoutHeight)),
                    skinLayout.VerticalScrollBarWidth,
                    (int)(layoutHeight / (float)_renderTarget.Height * layoutHeight));
               
                if (mousePressEvent != null && mousePressEvent.Button == MouseButton.Left)
                {
                    if (horizontalScrollBarRectangle.Contains(mousePressEvent.MouseState.Position))
                    {
                        if (_renderTarget.Width > layout.Width)
                        {
                            _isHorizontalScrolling = true;
                            _horizontalScrollOffset = mousePressEvent.MouseState.Position.X - horizontalScrollBarRectangle.X;
                            _horizontalScrollStart = mousePressEvent.MouseState.Position.X;
                        }

                        return true;
                    }

                    if (verticalScrollBarRectangle.Contains(mousePressEvent.MouseState.Position))
                    {
                        if (_renderTarget.Height > layout.Height)
                        {
                            _isVerticalScrolling = true;
                            _verticalScrollOffset = mousePressEvent.MouseState.Position.Y - verticalScrollBarRectangle.Y;
                            _verticalScrollStart = mousePressEvent.MouseState.Position.Y;
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
                    scrollXPixels = (int)(ScrollX * (Math.Max(_renderTarget.Width, layoutWidth) - layoutWidth));
                    scrollYPixels = (int)(ScrollY * (Math.Max(_renderTarget.Height, layoutHeight) - layoutHeight));

                    originalState = mouseEvent.MouseState;
                    mouseEvent.MouseState = new MouseState(
                        mouseEvent.MouseState.X + scrollXPixels,
                        mouseEvent.MouseState.Y + scrollYPixels,
                        mouseEvent.MouseState.ScrollWheelValue,
                        mouseEvent.MouseState.LeftButton,
                        mouseEvent.MouseState.MiddleButton,
                        mouseEvent.MouseState.RightButton,
                        mouseEvent.MouseState.XButton1,
                        mouseEvent.MouseState.XButton2);

                    var mouseMoveEvent = @event as MouseMoveEvent;

                    if (mouseMoveEvent != null)
                    {
                        mouseMoveEvent.LastX += scrollXPixels;
                        mouseMoveEvent.LastY += scrollYPixels;
                        mouseMoveEvent.X += scrollXPixels;
                        mouseMoveEvent.Y += scrollYPixels;
                    }
                }

                _child.HandleEvent(skinLayout, layout, context, @event);

                // Restore event state.
                if (mouseEvent != null)
                {
                    mouseEvent.MouseState = originalState;

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

