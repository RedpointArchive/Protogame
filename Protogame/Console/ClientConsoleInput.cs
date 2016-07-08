using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Protoinject;

namespace Protogame
{
    public class ClientConsoleInput : IConsoleInput
    {
        private readonly IKeyboardStringReader _keyboardStringReader;
        private readonly IKernel _kernel;
        private ICommand[] _commands;
        
        private StringBuilder _inputBuffer = new StringBuilder();

        public ClientConsoleInput(
            IKeyboardStringReader keyboardStringReader,
            IKernel kernel)
        {
            _keyboardStringReader = keyboardStringReader;
            _kernel = kernel;
        }

        public void Update(IGameContext gameContext, IUpdateContext updateContext, Action<string> logInternal)
        {
            if (_commands == null)
            {
                _commands = _kernel.GetAll<ICommand>();
            }

            var state = Keyboard.GetState();

            _keyboardStringReader.Process(state, gameContext.GameTime, this._inputBuffer);
            if (this._inputBuffer.ToString().LastIndexOf('`') != -1)
            {
                this._inputBuffer.Remove(this._inputBuffer.ToString().LastIndexOf('`'), 1);
            }

            if (state.IsKeyChanged(this, Keys.Enter) == KeyState.Down)
            {
                logInternal("> " + this._inputBuffer);
                this.Execute(gameContext, this.Parse(this._inputBuffer.ToString()), logInternal);
                this._inputBuffer = new StringBuilder();
            }
        }

        public StringBuilder InputBuffer => _inputBuffer;
        
        private void Execute(IGameContext gameContext, ParsedCommand parsedCommand, Action<string> logInternal)
        {
            foreach (var command in _commands)
            {
                if (command.Names.Contains(parsedCommand.Name))
                {
                    var output = command.Execute(gameContext, parsedCommand.Name, parsedCommand.Parameters.ToArray());
                    var lines = output.Split('\n');
                    int start = -1, end = lines.Length;
                    for (var i = 0; i < lines.Length; i++)
                    {
                        if (!string.IsNullOrEmpty(lines[i]))
                        {
                            start = i;
                            break;
                        }
                    }

                    if (start == -1)
                    {
                        return;
                    }

                    for (var i = lines.Length - 1; i >= start; i--)
                    {
                        if (string.IsNullOrEmpty(lines[i]))
                        {
                            end = i;
                            break;
                        }
                    }

                    for (var i = start; i < end; i++)
                    {
                        logInternal(lines[i]);
                    }

                    return;
                }
            }

            logInternal("No such command found.");
        }
        
        private ParsedCommand Parse(string input)
        {
            var parsed = new ParsedCommand();
            var split = input.Split(new[] { ' ' }, 2);
            parsed.Name = split[0];
            parsed.Parameters = new List<string>();
            if (split.Length > 1)
            {
                var inQuote = false;
                var buffer = string.Empty;
                for (var i = 0; i < split[1].Length; i++)
                {
                    if (split[1][i] == '"')
                    {
                        if (i == 0 || split[1][i - 1] != '\\')
                        {
                            inQuote = !inQuote;
                        }

                        if (!inQuote)
                        {
                            parsed.Parameters.Add(buffer);
                        }
                    }
                    else if (split[1][i] == ' ' && !inQuote)
                    {
                        parsed.Parameters.Add(buffer);
                        buffer = string.Empty;
                    }
                    else
                    {
                        buffer += split[1][i];
                    }
                }

                if (buffer.Length > 0)
                {
                    parsed.Parameters.Add(buffer);
                    buffer = string.Empty;
                }
            }

            return parsed;
        }
        
        private struct ParsedCommand
        {
            public string Name;
            
            public List<string> Parameters;
        }
    }
}
