using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jitter;
using Jitter.LinearMath;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Protogame
{
    public class PhysicsDebugDraw : IDebugDrawer
    {
        private readonly GraphicsDevice _graphicsDevice;
        private readonly bool _isRigidBodyActive;

        public PhysicsDebugDraw(GraphicsDevice graphicsDevice, bool isRigidBodyActive)
        {
            _graphicsDevice = graphicsDevice;
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
            _graphicsDevice.DrawUserPrimitives(
                PrimitiveType.TriangleList,
                new[]
                {
                    new VertexPositionNormalColor(pos1.ToXNAVector(), Vector3.Zero, _isRigidBodyActive ? Color.Red : Color.DarkRed), 
                    new VertexPositionNormalColor(pos2.ToXNAVector(), Vector3.Zero, _isRigidBodyActive ? Color.Green : Color.DarkGreen),
                    new VertexPositionNormalColor(pos3.ToXNAVector(), Vector3.Zero, _isRigidBodyActive ? Color.Blue : Color.DarkBlue)
                },
                0,
                1);
        }
    }
}
