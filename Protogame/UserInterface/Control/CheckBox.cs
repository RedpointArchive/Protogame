using System;
using Microsoft.Xna.Framework;

namespace Protogame
{
    public class CheckBox : IContainer
    {
        private bool _shouldAppearPressedWhenMouseIsOver;
        private Rectangle _mouseDownLayout;

        public CheckBox()
        {
        }

        public event EventHandler Changed;

        public IContainer[] Children => IContainerConstant.EmptyContainers;

        public bool Focused { get; set; }

        public int Order { get; set; }

        public IContainer Parent { get; set; }

        public object Userdata { get; set; }

        public bool Checked { get; private set; }

        public bool IsDown { get; private set; }

        public virtual void Render(IRenderContext context, ISkinLayout skinLayout, ISkinDelegator skinDelegator, Rectangle layout)
        {
            skinDelegator.Render(context, layout, this);
        }

        public void Update(ISkinLayout skin, Rectangle layout, GameTime gameTime, ref bool stealFocus)
        {
        }

        public bool HandleEvent(ISkinLayout skin, Rectangle layout, IGameContext context, Event @event)
        {
            var mouseEvent = @event as MouseEvent;
            var mousePressEvent = @event as MousePressEvent;
            var mouseReleaseEvent = @event as MouseReleaseEvent;
            var mouseMoveEvent = @event as MouseMoveEvent;

            if (mouseEvent == null)
            {
                return false;
            }

            var x = 0;
            var y = 0;

            if (mouseEvent != null)
            {
                x = mouseEvent.MouseState.X;
                y = mouseEvent.MouseState.Y;
            }

            if (layout.Contains(x, y))
            {
                if (mouseMoveEvent != null)
                {
                    if (_shouldAppearPressedWhenMouseIsOver)
                    {
                        IsDown = true;
                    }
                }
                else if (mousePressEvent != null && mousePressEvent.Button == MouseButton.Left)
                {
                    _shouldAppearPressedWhenMouseIsOver = true;
                    _mouseDownLayout = layout;
                    IsDown = true;
                    this.Focus();
                    return true;
                }
                else if (mouseReleaseEvent != null && mouseReleaseEvent.Button == MouseButton.Left)
                {
                    if (_mouseDownLayout == layout)
                    {
                        Checked = !Checked;
                        Changed?.Invoke(this, new EventArgs());

                        _shouldAppearPressedWhenMouseIsOver = false;
                    }

                    IsDown = false;
                    this.Focus();
                }
            }
            else if (mouseMoveEvent != null)
            {
                IsDown = false;
            }

            if (mouseReleaseEvent != null && mouseReleaseEvent.Button == MouseButton.Left)
            {
                IsDown = false;
                _shouldAppearPressedWhenMouseIsOver = false;
            }

            return false;
        }
    }
}