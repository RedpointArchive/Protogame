using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
#if PLATFORM_WINDOWS || PLATFORM_MACOS || PLATFORM_LINUX
using System.Windows.Forms;
#endif

namespace Protogame
{
    public class FileSelect : IContainer
    {
        public IContainer[] Children { get { return new IContainer[0]; } }
        public IContainer Parent { get; set; }
        public int Order { get; set; }
        public ButtonUIState State { get; private set; }
        public string Path { get; set; }
        public bool Focused { get; set; }
        public event EventHandler Changed;

        public FileSelect()
        {
            this.State = ButtonUIState.None;
        }

        public void Update(ISkin skin, Rectangle layout, GameTime gameTime, ref bool stealFocus)
        {
            var mouse = Mouse.GetState();
            var leftPressed = mouse.LeftPressed(this);
            if (layout.Contains(mouse.X, mouse.Y))
            {
                if (leftPressed)
                {
                    if (this.State != ButtonUIState.Clicked)
                    {
                        this.State = ButtonUIState.Clicked;
                        this.Focus();

						#if PLATFORM_WINDOWS || PLATFORM_MACOS || PLATFORM_LINUX
                        using (var openFileDialog = new OpenFileDialog())
                        {
                            if (openFileDialog.ShowDialog() == DialogResult.OK)
                            {
                                this.Path = openFileDialog.FileName;
                                if (this.Changed != null)
                                    this.Changed(this, new EventArgs());
                            }
                            Application.DoEvents();
                        }
						#endif
                    }
                }
                else
                    this.State = ButtonUIState.Hover;
            }
            else
                this.State = ButtonUIState.None;
        }

        public void Draw(IRenderContext context, ISkin skin, Rectangle layout)
        {
            skin.DrawFileSelect(context, layout, this);
        }
    }
}
