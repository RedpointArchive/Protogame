namespace Protogame
{
    using System;

    /// <summary>
    /// The exit command.
    /// </summary>
    public class ExitCommand : ICommand
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
                return new[] { null, "Exit the game." };
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
                return new[] { "quit", "exit" };
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
            Environment.Exit(0);
            return string.Empty;
        }
    }
}