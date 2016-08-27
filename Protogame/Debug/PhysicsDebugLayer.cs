// ReSharper disable CheckNamespace

namespace Protogame
{
    /// <summary>
    /// When this debug layer is added to a debug render pass, the physics engine will
    /// render the physics meshes and the state of all rigid bodies in the system to
    /// the screen.  To enable this functionality, create a new instance of
    /// <see cref="PhysicsDebugLayer"/> and add it to the <see cref="IDebugRenderPass.EnabledLayers"/>
    /// list on the render pass.
    /// </summary>
    /// <module>Debug</module>
    public class PhysicsDebugLayer : IDebugLayer
    {
    }
}
