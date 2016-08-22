using System;
#if PLATFORM_WINDOWS || PLATFORM_MACOS || PLATFORM_LINUX
using System.Windows.Forms;
#endif
using Microsoft.Xna.Framework;

namespace Protogame
{	
    public class FileSelect : IContainer
    {
        public FileSelect()
        {
            State = ButtonUIState.None;
        }
        
        public event EventHandler Changed;

        public IContainer[] Children => IContainerConstant.EmptyContainers;

        public bool Focused { get; set; }
        
        public int Order { get; set; }
        
        public IContainer Parent { get; set; }

        public object Userdata { get; set; }
        
        public string Path { get; set; }
        
        public ButtonUIState State { get; private set; }
        
        public void Render(IRenderContext context, ISkinLayout skinLayout, ISkinDelegator skinDelegator, Rectangle layout)
        {
            skinDelegator.Render(context, layout, this);
        }
        
        public void Update(ISkinLayout skinLayout, Rectangle layout, GameTime gameTime, ref bool stealFocus)
        {
        }
        
        public bool HandleEvent(ISkinLayout skinLayout, Rectangle layout, IGameContext context, Event @event)
        {
            var mouseEvent = @event as MouseEvent;
            var mousePressEvent = @event as MousePressEvent;
            var mouseReleaseEvent = @event as MouseReleaseEvent;
            var mouseMoveEvent = @event as MouseMoveEvent;

            if (mouseEvent == null)
            {
                return false;
            }

            if (layout.Contains(mouseEvent.MouseState.X, mouseEvent.MouseState.Y))
            {
                if (mouseMoveEvent != null)
                {
                    State = ButtonUIState.Hover;
                }
                else if (mousePressEvent != null && mousePressEvent.Button == MouseButton.Left)
                {
                    State = ButtonUIState.Clicked;
                    this.Focus();

#if PLATFORM_WINDOWS || PLATFORM_MACOS || PLATFORM_LINUX
                    using (var openFileDialog = new OpenFileDialog())
                    {
                        if (openFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            Path = openFileDialog.FileName;
                            if (Changed != null)
                            {
                                Changed(this, new EventArgs());
                            }
                        }

                        Application.DoEvents();
                    }
#endif

                    return true;
                }
            }
            else
            {
                State = ButtonUIState.None;
            }

            if (mouseReleaseEvent != null && mouseReleaseEvent.Button == MouseButton.Left)
            {
                State = ButtonUIState.None;
            }

            return false;
        }
    }
}