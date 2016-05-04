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

        public PhysicsDebugDraw(GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;
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
                    new VertexPositionColor(pos1.ToXNAVector(), Color.Red), 
                    new VertexPositionColor(pos2.ToXNAVector(), Color.Green),
                    new VertexPositionColor(pos3.ToXNAVector(), Color.Blue)
                },
                0,
                1);
        }
    }
}
