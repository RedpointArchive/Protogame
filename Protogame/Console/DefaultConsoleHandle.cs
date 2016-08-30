using Protoinject;

namespace Protogame
{
    public class DefaultConsoleHandle : IConsoleHandle
    {
        private readonly INode _node;
        private readonly IKernel _kernel;

        private IConsole _console;

        public DefaultConsoleHandle(INode node, IKernel kernel)
        {
            _node = node;
            _kernel = kernel;
        }

        public void Log(string messageFormat)
        {
            if (_console == null)
            {
                _console = _kernel.Get<IConsole>();
            }

            _console.LogStructured(_node.Parent, ConsoleLogLevel.Debug, messageFormat, null);
        }

        public void Log(string messageFormat, params object[] objects)
        {
            if (_console == null)
            {
                _console = _kernel.Get<IConsole>();
            }

            _console.LogStructured(_node.Parent, ConsoleLogLevel.Debug, messageFormat, objects);
        }

        public void LogDebug(string messageFormat)
        {
            if (_console == null)
            {
                _console = _kernel.Get<IConsole>();
            }

            _console.LogStructured(_node.Parent, ConsoleLogLevel.Debug, messageFormat, null);
        }

        public void LogDebug(string messageFormat, params object[] objects)
        {
            if (_console == null)
            {
                _console = _kernel.Get<IConsole>();
            }

            _console.LogStructured(_node.Parent, ConsoleLogLevel.Debug, messageFormat, objects);
        }

        public void LogInfo(string messageFormat)
        {
            if (_console == null)
            {
                _console = _kernel.Get<IConsole>();
            }

            _console.LogStructured(_node.Parent, ConsoleLogLevel.Info, messageFormat, null);
        }

        public void LogInfo(string messageFormat, params object[] objects)
        {
            if (_console == null)
            {
                _console = _kernel.Get<IConsole>();
            }

            _console.LogStructured(_node.Parent, ConsoleLogLevel.Info, messageFormat, objects);
        }

        public void LogWarning(string messageFormat)
        {
            if (_console == null)
            {
                _console = _kernel.Get<IConsole>();
            }

            _console.LogStructured(_node.Parent, ConsoleLogLevel.Warning, messageFormat, null);
        }

        public void LogWarning(string messageFormat, params object[] objects)
        {
            if (_console == null)
            {
                _console = _kernel.Get<IConsole>();
            }

            _console.LogStructured(_node.Parent, ConsoleLogLevel.Warning, messageFormat, objects);
        }

        public void LogError(string messageFormat)
        {
            if (_console == null)
            {
                _console = _kernel.Get<IConsole>();
            }

            _console.LogStructured(_node.Parent, ConsoleLogLevel.Error, messageFormat, null);
        }

        public void LogError(string messageFormat, params object[] objects)
        {
            if (_console == null)
            {
                _console = _kernel.Get<IConsole>();
            }

            _console.LogStructured(_node.Parent, ConsoleLogLevel.Error, messageFormat, objects);
        }
    }
}