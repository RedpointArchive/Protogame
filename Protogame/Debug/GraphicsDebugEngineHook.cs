namespace Protogame
{
    public class GraphicsDebugEngineHook : IEngineHook
    {
        private readonly IConsoleHandle _consoleHandle;

        public GraphicsDebugEngineHook(IConsoleHandle consoleHandle)
        {
            _consoleHandle = consoleHandle;
        }

        public void Render(IGameContext gameContext, IRenderContext renderContext)
        {
#if PLATFORM_WINDOWS
            Microsoft.Xna.Framework.Graphics.GraphicsDebugMessage message;

            if (renderContext.GraphicsDevice.GraphicsDebug != null)
            {
                while (renderContext.GraphicsDevice.GraphicsDebug.TryDequeueMessage(out message))
                {
                    switch (message.Severity)
                    {
                        case "Corruption":
                        case "Error":
                            _consoleHandle.LogError("DX11 ({0}) ({1}, {2}): {3}", message.Severity, message.Category, message.IdName, message.Message);
                            break;
                        case "Warning":
                            _consoleHandle.LogWarning("DX11 ({0}) ({1}, {2}): {3}", message.Severity, message.Category, message.IdName, message.Message);
                            break;
                        case "Information":
                            _consoleHandle.LogDebug("DX11 ({0}) ({1}, {2}): {3}", message.Severity, message.Category, message.IdName, message.Message);
                            break;
                    }
                }
            }
#endif
        }

        public void Update(IGameContext gameContext, IUpdateContext updateContext)
        {
        }

        public void Update(IServerContext serverContext, IUpdateContext updateContext)
        {
        }
    }
}
