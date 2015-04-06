using Gtk;
using Protogame;
using Ninject;
using Microsoft.Xna.Framework;
using ProtogameEditor;

namespace ProtogameEditorHost
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            Application.Init();

            var window = new HostWindow();
            window.Show();

            var editor = new IDEEditor();

            var kernel = editor.CreateEditorKernel<EditorHostEmbedContext>();

            var context = kernel.Get<IEmbedContext>();

            EditorGame game = null;

            while (true)
            {
                Application.RunIteration();

                if (window.GLWidget.WindowInfo != null && game == null)
                {
                    ((EditorHostEmbedContext)context).GraphicsContext = window.GLWidget.GraphicsContext;
                    ((EditorHostEmbedContext)context).WindowInfo = window.GLWidget.WindowInfo;
                    ((EditorHostEmbedContext)context).GLWidget = window.GLWidget;

                    game = new EditorGame(kernel);
                }

                if (game != null)
                {
                    game.RunOneFrame();
                }
            }
        }
    }
}