using System;

namespace Protogame
{
    public class ExitCommand : ICommand
    {
        public string[] Names { get { return new[] { "quit", "exit" }; } }
        public string[] Descriptions
        {
            get
            {
                return new[]
                {
                    null,
                    "Exit the game."
                };
            }
        }

        public string Execute(IGameContext gameContext, string name, string[] parameters)
        {
            Environment.Exit(0);
            return "";
        }
    }
}

