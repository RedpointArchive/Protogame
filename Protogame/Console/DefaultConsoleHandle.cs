using Protoinject;

namespace Protogame
{
    public class DefaultConsoleHandle : IConsoleHandle
    {
        private readonly INode _node;
        private readonly IConsole _console;

        public DefaultConsoleHandle(INode node, IConsole console)
        {
            _node = node;
            _console = console;
        }

        public void Log(string messageFormat)
        {
            _console.LogStructured(_node.Parent, messageFormat, null);
        }

        public void Log(string messageFormat, params object[] objects)
        {
            _console.LogStructured(_node.Parent, messageFormat, objects);
        }
    }
}