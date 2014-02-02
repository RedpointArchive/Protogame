namespace Protogame
{
    /// <summary>
    /// The Command interface.
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// Gets the descriptions.
        /// </summary>
        /// <value>
        /// The descriptions.
        /// </value>
        string[] Descriptions { get; }

        /// <summary>
        /// Gets the names.
        /// </summary>
        /// <value>
        /// The names.
        /// </value>
        string[] Names { get; }

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
        string Execute(IGameContext gameContext, string name, string[] parameters);
    }
}