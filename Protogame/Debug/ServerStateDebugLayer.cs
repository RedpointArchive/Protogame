// ReSharper disable CheckNamespace

namespace Protogame
{
    /// <summary>
    /// When this debug layer is added to a debug render pass, the network engine and
    /// network synchronisation components will render the state of the server, the state
    /// of the client, and interpolated information to the screen.  To enable this
    /// functionality, create a new instance of <see cref="ServerStateDebugLayer"/> and add
    /// it to the <see cref="IDebugRenderPass.EnabledLayers"/> list on the render pass.
    /// </summary>
    /// <module>Debug</module>
    public class ServerStateDebugLayer : IDebugLayer
    {
    }
}
