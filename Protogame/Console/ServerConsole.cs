using System;
using Protoinject;

namespace Protogame
{
    public class ServerConsole : IConsole
    {
        public bool Open => true;

        public void Render(IGameContext gameContext, IRenderContext renderContext)
        {
            throw new NotSupportedException();
        }

        public void Toggle()
        {
            throw new NotSupportedException();
        }

        public void Update(IGameContext gameContext, IUpdateContext updateContext)
        {
            throw new NotSupportedException();
        }

        public void Log(string message)
        {
            Console.WriteLine(message);
        }

        public void LogStructured(INode node, string format, object[] args)
        {
            if (args == null)
            {
                Console.WriteLine(format);
            }
            else
            {
                Console.WriteLine(format, args);
            }
        }
    }
}