using System;

namespace Example
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (ExampleGame game = new ExampleGame())
            {
                game.Run();
            }
        }
    }
#endif
}

