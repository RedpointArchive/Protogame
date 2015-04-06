using System;
using Microsoft.Xna.Framework;
using Gtk;

namespace ProtogameEditor
{
    public class EditorHostEmbedContext : IEmbedContext
    {
        private Gtk.Widget _glWidget;

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

        public Gtk.Widget GLWidget
        {
            get
            {
                return this._glWidget;
            }
            set
            {
                if (this._glWidget != null)
                {
                    this._glWidget.ConfigureEvent -= this.Resize;
                }

                this._glWidget = value;

                if (this._glWidget != null)
                {
                    this._glWidget.ConfigureEvent += this.Resize;
                }
            }
        }

        public System.Drawing.Point PointToClient(System.Drawing.Point point)
        {
            int x, y;
            if (this.GLWidget.GdkWindow == null)
            {
                return new System.Drawing.Point(0, 0);
            }

            this.GLWidget.GdkWindow.GetOrigin(out x, out y);
            return new System.Drawing.Point(
                point.X - x,
                point.Y - y);
        }

        public Rectangle GetClientBounds()
        {
            return new Rectangle(
                0,
                0,
                this.GLWidget.Allocation.Width,
                this.GLWidget.Allocation.Height);
        }

        public event System.EventHandler OnResize;

        public void Resize(object sender, ConfigureEventArgs e)
        {
            if (this.OnResize != null)
            {
                this.OnResize(this, new System.EventArgs());
            }
        }
    }
}

