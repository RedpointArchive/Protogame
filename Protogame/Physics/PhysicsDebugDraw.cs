using Jitter;
using Jitter.LinearMath;
using Microsoft.Xna.Framework;

namespace Protogame
{
    public class PhysicsDebugDraw : IDebugDrawer
    {
        private readonly IRenderContext _renderContext;
        private readonly IDebugRenderer _debugRenderer;
        private readonly bool _isRigidBodyActive;

        public PhysicsDebugDraw(IRenderContext renderContext, IDebugRenderer debugRenderer, bool isRigidBodyActive)
        {
            _renderContext = renderContext;
            _debugRenderer = debugRenderer;
            _isRigidBodyActive = isRigidBodyActive;
        }

        public void DrawLine(JVector start, JVector end)
        {
            
        }

        public void DrawPoint(JVector pos)
        {
            
        }

        public void DrawTriangle(JVector pos1, JVector pos2, JVector pos3)
        {
            _debugRenderer.RenderDebugTriangle(
                _renderContext,
                pos1.ToXNAVector(),
                pos2.ToXNAVector(),
                pos3.ToXNAVector(),
                _isRigidBodyActive ? Color.Red : Color.DarkRed,
                _isRigidBodyActive ? Color.Green : Color.DarkGreen,
                _isRigidBodyActive ? Color.Blue : Color.DarkBlue);
        }
    }
}
