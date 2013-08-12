using System;

namespace Protogame
{
    public class HelpCommand : ICommand
    {
        private Lazy<ICommand[]> m_Commands;
    
        public string[] Names { get { return new[] { "help" }; } }
        public string[] Descriptions
        {
            get
            {
                return new[]
                {
                    "Show this help."
                };
            }
        }
        
        public HelpCommand(
            Lazy<ICommand[]> commands)
        {
            this.m_Commands = commands;
        }

        public string Execute(IGameContext gameContext, string name, string[] parameters)
        {
            var buffer = "";
            foreach (var command in this.m_Commands.Value)
            {
                for (var i = 0; i < command.Names.Length; i++)
                {
                    if (command.Descriptions[i] == null)
                        continue;
                    buffer += command.Names[i] + " - " + command.Descriptions[i] + "\n";
                }
            }
            return buffer;
        }
    }
}

