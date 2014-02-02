namespace Protogame
{
    using System;

    /// <summary>
    /// The gc command.
    /// </summary>
    public class GCCommand : ICommand
    {
        /// <summary>
        /// Gets the descriptions.
        /// </summary>
        /// <value>
        /// The descriptions.
        /// </value>
        public string[] Descriptions
        {
            get
            {
                return new[] { "Control garbage collection." };
            }
        }

        /// <summary>
        /// Gets the names.
        /// </summary>
        /// <value>
        /// The names.
        /// </value>
        public string[] Names
        {
            get
            {
                return new[] { "gc" };
            }
        }

        /// <summary>
        /// The execute.
        /// </summary>
        /// <param name="gameContext">
        /// The game context.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string Execute(IGameContext gameContext, string name, string[] parameters)
        {
            if (parameters.Length < 1)
            {
                return "Not enough parameters.";
            }

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