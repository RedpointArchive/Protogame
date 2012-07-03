using System;
using Process4.Attributes;
using Process4;

namespace Example.RTS
{
#if WINDOWS || XBOX
    [Distributed(Architecture.ServerClient, Caching.PushOnChange)]
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            LocalNode node = new LocalNode();
            node.Join();

            using (RuntimeGame game = new RuntimeGame())
            {
                game.Run();
            }

            node.Leave();
        }
    }
#endif
}

