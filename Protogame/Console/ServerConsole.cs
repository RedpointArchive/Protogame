using System;
using Protoinject;

namespace Protogame
{
    public class ServerConsole : IConsole
    {
        private readonly ILogShipping _logShipping;

        public ServerConsole(ILogShipping logShipping)
        {
            _logShipping = logShipping;
        }

        public ConsoleState State => ConsoleState.OpenNoInput;

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
            _logShipping.AddLog(new PendingLogForShip { Message = message, LogLevel = ConsoleLogLevel.Debug });

            Console.WriteLine(message);
        }

        public void LogStructured(INode node, string format, object[] args)
        {
            _logShipping.AddLog(new PendingLogForShip { Message = args == null ? format : string.Format(format, args), LogLevel = ConsoleLogLevel.Debug });

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
            _logShipping.AddLog(new PendingLogForShip { Message = args == null ? format : string.Format(format, args), LogLevel = logLevel });

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