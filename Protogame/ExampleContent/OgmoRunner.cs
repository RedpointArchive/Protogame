using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Protogame.ExampleContent
{
    /// <summary>
    /// In order to use this class you will need to copy this class and OgmoState to
    /// your game (don't uncomment it here!).  Your game will also need to reference
    /// the OgmoEditor.Protogame assembly that comes with Ogmo Editor.
    /// </summary>
#if EXAMPLE
    public class OgmoRunner : IOgmoRunner
    {
        public object Start(string level)
        {
            LocalNode node = new LocalNode(Assembly.GetExecutingAssembly());
            node.Join();

            try
            {
                FileInfo levelInfo = new FileInfo(level);
                if (levelInfo.Directory.Name != "Resources")
                    return "Level is not in a resources directory.";

                DirectoryInfo directoryInfo = levelInfo.Directory.Parent;
                World.BaseDirectory = directoryInfo.FullName;
                World.RuntimeDirectory = new FileInfo(Assembly.GetExecutingAssembly().Location).Directory.FullName;

                using (RuntimeGame game = new RuntimeGame(levelInfo.Name.Substring(0, levelInfo.Name.Length - levelInfo.Extension.Length)))
                {
                    game.Run();
                }
            }
            finally
            {
                node.Leave();
            }

            return null;
        }
    }
#endif
}
