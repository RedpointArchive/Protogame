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

        public void LogStructured(INode node, ConsoleLogLevel logLevel, string format, object[] args)
        {
            switch (logLevel)
            {
                case ConsoleLogLevel.Debug:
                    Console.ForegroundColor = ConsoleColor.Gray;
                    break;
                case ConsoleLogLevel.Info:
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    break;
                case ConsoleLogLevel.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case ConsoleLogLevel.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
            }

            if (args == null)
            {
                Console.WriteLine(format);
            }
            else
            {
                Console.WriteLine(format, args);
            }

            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}