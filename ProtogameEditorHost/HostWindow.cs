using System;
using Gtk;

namespace ProtogameEditorHost
{
    public partial class HostWindow : Gtk.Window
    {
        public HostWindow() :
            base(Gtk.WindowType.Toplevel)
        {
            this.Build();
        }

        public GLWidget GLWidget
        {
            get { return glWidget; }
        }
    }
}

