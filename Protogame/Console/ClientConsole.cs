using System;
using System.Linq;
using Protoinject;

namespace Protogame
{
    using System.Collections.Generic;

    public class ClientConsole : IConsole
    {
        private readonly IConsoleInput _consoleInput;
        private readonly IConsoleRender _consoleRender;

        private readonly List<ConsoleEntry> _log = new List<ConsoleEntry>();
        
        public ClientConsole(
            IConsoleInput consoleInput,
            IConsoleRender consoleRender)
        {
            _consoleInput = consoleInput;
            _consoleRender = consoleRender;
        }

        public bool Open { get; private set; }
        
        public void Render(IGameContext gameContext, IRenderContext renderContext)
        {
            if (!Open)
            {
                return;
            }

            _consoleRender.Render(
                gameContext,
                renderContext,
                _consoleInput.InputBuffer,
                _log.Select(x => new Tuple<ConsoleLogLevel, string>(x.LogLevel, x.Name == string.Empty ? x.Message : $"<{x.Name,-20}> ({x.Count,5}) {x.Message}")).ToList());

            if (_log.Count > 31)
            {
                _log.RemoveRange(0, _log.Count - 31);
            }
        }
        
        public void Toggle()
        {
            Open = !Open;
        }
        
        public void Update(IGameContext gameContext, IUpdateContext updateContext)
        {
            if (!Open)
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

        private class ConsoleEntry
        {
            public string Name { get; set; }

            public int Count { get; set; }

            public string Message { get; set; }

            public ConsoleLogLevel LogLevel { get; set; }
        }
    }
}