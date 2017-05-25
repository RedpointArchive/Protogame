using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace Protogame
{
    public class Button : IContainer
    {
        private bool _shouldAppearPressedWhenMouseIsOver;

        public Button()
        {
            State = ButtonUIState.None;
        }

        public event EventHandler Click;

        public IContainer[] Children => IContainerConstant.EmptyContainers;

        public bool Focused { get; set; }

        public int Order { get; set; }

        public IContainer Parent { get; set; }

        public object Userdata { get; set; }

        public ButtonUIState State { get; private set; }

        public string Text { get; set; }

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
            var touchEvent = @event as TouchEvent;
            var touchPressEvent = @event as TouchPressEvent;
            var touchHeldEvent = @event as TouchHeldEvent;
            var touchReleaseEvent = @event as TouchReleaseEvent;

            if (mouseEvent == null && touchEvent == null)
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

            if (touchPressEvent != null)
            {
                x = (int)touchPressEvent.X;
                y = (int)touchPressEvent.Y;
            }

            if (touchReleaseEvent != null)
            {
                x = (int)touchReleaseEvent.X;
                y = (int)touchReleaseEvent.Y;
            }

            if (touchHeldEvent != null)
            {
                x = (int)touchHeldEvent.X;
                y = (int)touchHeldEvent.Y;
            }

            if (layout.Contains(x, y))
            {
                if (mouseMoveEvent != null)
                {
                    if (_shouldAppearPressedWhenMouseIsOver)
                    {
                        State = ButtonUIState.Clicked;
                    }
                    else
                    {
                        State = ButtonUIState.Hover;
                    }
                }
                else if (mousePressEvent != null && mousePressEvent.Button == MouseButton.Left)
                {
                    _shouldAppearPressedWhenMouseIsOver = true;
                    State = ButtonUIState.Clicked;
                    this.Focus();
                    return true;
                }
                else if (mouseReleaseEvent != null && mouseReleaseEvent.Button == MouseButton.Left)
                {
                    if (_shouldAppearPressedWhenMouseIsOver)
                    {
                        if (Click != null && State == ButtonUIState.Clicked)
                        {
                            Click(this, new EventArgs());
                        }

                        _shouldAppearPressedWhenMouseIsOver = false;
                    }

                    State = ButtonUIState.None;
                    this.Focus();
                }
            }
            else if (mouseMoveEvent != null)
            {
                State = ButtonUIState.None;
            }

            if (mouseReleaseEvent != null && mouseReleaseEvent.Button == MouseButton.Left)
            {
                State = ButtonUIState.None;
                _shouldAppearPressedWhenMouseIsOver = false;
            }

            if (touchReleaseEvent != null && layout.Contains(x, y))
            {
                State = ButtonUIState.None;
                _shouldAppearPressedWhenMouseIsOver = false;
            }

            return false;
        }
    }
}