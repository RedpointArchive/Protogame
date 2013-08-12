using System;
using Ninject;

namespace Protogame
{
    public class HelpCommand : ICommand
    {
        private IKernel m_Kernel;
    
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
            IKernel kernel)
        {
            this.m_Kernel = kernel;
        }

        public string Execute(IGameContext gameContext, string name, string[] parameters)
        {
            var buffer = "";
            foreach (var command in this.m_Kernel.GetAll<ICommand>())
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

