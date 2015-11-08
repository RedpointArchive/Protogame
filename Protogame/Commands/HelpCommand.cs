namespace Protogame
{
    using Protoinject;

    /// <summary>
    /// The help command.
    /// </summary>
    public class HelpCommand : ICommand
    {
        /// <summary>
        /// The m_ kernel.
        /// </summary>
        private readonly IKernel m_Kernel;

        /// <summary>
        /// Initializes a new instance of the <see cref="HelpCommand"/> class.
        /// </summary>
        /// <param name="kernel">
        /// The kernel.
        /// </param>
        public HelpCommand(IKernel kernel)
        {
            this.m_Kernel = kernel;
        }

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
                return new[] { "Show this help." };
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
                return new[] { "help" };
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
            var buffer = string.Empty;
            foreach (var command in this.m_Kernel.GetAll<ICommand>())
            {
                for (var i = 0; i < command.Names.Length; i++)
                {
                    if (command.Descriptions[i] == null)
                    {
                        continue;
                    }

                    buffer += command.Names[i] + " - " + command.Descriptions[i] + "\n";
                }
            }

            return buffer;
        }
    }
}