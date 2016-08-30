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
            _console.LogStructured(_node.Parent, ConsoleLogLevel.Debug, messageFormat, null);
        }

        public void Log(string messageFormat, params object[] objects)
        {
            _console.LogStructured(_node.Parent, ConsoleLogLevel.Debug, messageFormat, objects);
        }

        public void LogDebug(string messageFormat)
        {
            _console.LogStructured(_node.Parent, ConsoleLogLevel.Debug, messageFormat, null);
        }

        public void LogDebug(string messageFormat, params object[] objects)
        {
            _console.LogStructured(_node.Parent, ConsoleLogLevel.Debug, messageFormat, objects);
        }

        public void LogInfo(string messageFormat)
        {
            _console.LogStructured(_node.Parent, ConsoleLogLevel.Info, messageFormat, null);
        }

        public void LogInfo(string messageFormat, params object[] objects)
        {
            _console.LogStructured(_node.Parent, ConsoleLogLevel.Info, messageFormat, objects);
        }

        public void LogWarning(string messageFormat)
        {
            _console.LogStructured(_node.Parent, ConsoleLogLevel.Warning, messageFormat, null);
        }

        public void LogWarning(string messageFormat, params object[] objects)
        {
            _console.LogStructured(_node.Parent, ConsoleLogLevel.Warning, messageFormat, objects);
        }

        public void LogError(string messageFormat)
        {
            _console.LogStructured(_node.Parent, ConsoleLogLevel.Error, messageFormat, null);
        }

        public void LogError(string messageFormat, params object[] objects)
        {
            _console.LogStructured(_node.Parent, ConsoleLogLevel.Error, messageFormat, objects);
        }
    }
}