using System;
using Microsoft.Xna.Framework;

namespace ProtogameEditor
{
    public class EditorEmbedContext : IEmbedContext
    {
        public EditorEmbedContext()
        {
            this.Width = 400;
            this.Height = 300;
        }

		public IntPtr WindowHandle
		{
			get;
			set;
		}

#if PLATFORM_LINUX
        public OpenTK.Graphics.IGraphicsContext GraphicsContext
        {
            get;
            set;
        }

        public OpenTK.Platform.IWindowInfo WindowInfo
        {
            get;
            set;
        }
#endif

        public int Width { get; set; }

        public int Height { get; set; }

        public void TriggerResize()
        {
            if (this.OnResize != null)
            {
                this.OnResize(this, new EventArgs());
            }
        }

        public System.Drawing.Point PointToClient(System.Drawing.Point point)
        {
            return new System.Drawing.Point(0, 0);
        }

        public Rectangle GetClientBounds()
        {
            return new Rectangle(
                0,
                0,
                this.Width,
                this.Height);
        }

        public event System.EventHandler OnResize;
    }
}

