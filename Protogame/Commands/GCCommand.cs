using System;

namespace Protogame
{
    public class GCCommand : ICommand
    {
        public string[] Names { get { return new[] { "gc" }; } }
        public string[] Descriptions
        {
            get
            {
                return new[]
                {
                    "Control garbage collection."
                };
            }
        }

        public string Execute(IGameContext gameContext, string name, string[] parameters)
        {
            if (parameters.Length < 1)
                return "Not enough parameters.";

            switch (parameters[0].ToLower())
            {
                case "help":
                    return @"collect - Force a full garbage collection to occur.";
                case "collect":
                    GC.Collect();
                    return "Garbage collection complete.";
                default:
                    return "Unknown command (try `help`).";
            }
        }
    }
}

