using System;
using System.Linq;
using Protoinject;

namespace Protogame
{
    using System.Collections.Generic;

    public class ClientConsole : IConsole
    {
        private readonly ILogShipping _logShipping;
        private readonly IConsoleInput _consoleInput;
        private readonly IConsoleRender _consoleRender;

        private readonly List<ConsoleEntry> _log = new List<ConsoleEntry>();
        private readonly object _logLock = new object();
        
        public ClientConsole(
            ILogShipping logShipping,
            IConsoleInput consoleInput,
            IConsoleRender consoleRender)
        {
            _logShipping = logShipping;
            _consoleInput = consoleInput;
            _consoleRender = consoleRender;
        }

        public ConsoleState State { get; private set; }
        
        public void Render(IGameContext gameContext, IRenderContext renderContext)
        {
            if (State == ConsoleState.Closed)
            {
                return;
            }

            lock (_logLock)
            {
                if (_log.Count > 30)
                {
                    _log.RemoveRange(0, _log.Count - 30);
                }

                _consoleRender.Render(
                    gameContext,
                    renderContext,
                    _consoleInput.InputBuffer,
                    State,
                    _log.Select(x => new Tuple<ConsoleLogLevel, string>(x.LogLevel, x.Name == string.Empty ? x.Message : $"<{x.Name,-20}> ({x.Count,5}) {x.Message}")).ToList());
            }
        }
        
        public void Toggle()
        {
            switch (State)
            {
                case ConsoleState.Closed:
                    State = ConsoleState.Open;
                    break;
                case ConsoleState.Open:
                    State = ConsoleState.OpenNoInput;
                    break;
                case ConsoleState.OpenNoInput:
                    State = ConsoleState.FullOpen;
                    break;
                case ConsoleState.FullOpen:
                    State = ConsoleState.FullOpenNoInput;
                    break;
                case ConsoleState.FullOpenNoInput:
                    State = ConsoleState.Closed;
                    break;
            }
        }
        
        public void Update(IGameContext gameContext, IUpdateContext updateContext)
        {
            if (State == ConsoleState.Closed ||
                State == ConsoleState.OpenNoInput ||
                State == ConsoleState.FullOpenNoInput)
            {
                return;
            }

            _consoleInput.Update(gameContext, updateContext, Log);
        }

        public void Log(string message)
        {
            LogInternal(new ConsoleEntry { Count = 1, Message = message, Name = string.Empty });
        }

        public void LogStructured(INode node, string format, object[] args)
        {
            var name = string.IsNullOrWhiteSpace(node.Name) ? node.Type.Name : node.Name;

            if (name.Length > 20)
            {
                name = name.Substring(0, 17) + "...";
            }

            LogInternal(new ConsoleEntry { Count = 1, LogLevel = ConsoleLogLevel.Debug, Message = args == null ? format : string.Format(format, args), Name = name });
        }

        public void LogStructured(INode node, ConsoleLogLevel logLevel, string format, object[] args)
        {
            var name = string.IsNullOrWhiteSpace(node.Name) ? node.Type.Name : node.Name;

            if (name.Length > 20)
            {
                name = name.Substring(0, 17) + "...";
            }

            LogInternal(new ConsoleEntry { Count = 1, LogLevel = logLevel, Message = args == null ? format : string.Format(format, args), Name = name });
        }

        private void LogInternal(ConsoleEntry consoleEntry)
        {
            _logShipping.AddLog(new PendingLogForShip { Message = consoleEntry.Message, LogLevel = consoleEntry.LogLevel });

            lock (_logLock)
            {
                if (_log.Count > 0)
                {
                    var last = _log[_log.Count - 1];

                    if (last.Name != string.Empty && last.Name == consoleEntry.Name && last.Message == consoleEntry.Message)
                    {
                        last.Count++;
                    }
                    else
                    {
                        _log.Add(consoleEntry);
                    }
                }
                else
                {
                    _log.Add(consoleEntry);
                }
            }
        }

        private class ConsoleEntry
        {
            public string Name { get; set; }

            public int Count { get; set; }

            public string Message { get; set; }

            public ConsoleLogLevel LogLevel { get; set; }
        }
    }
}