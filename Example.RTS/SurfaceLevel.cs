using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Protogame;
using Protogame.MultiLevel;

namespace Example.RTS
{
    public class SurfaceLevel : PathFindingLevel
    {
        public SurfaceLevel(MultiLevelWorld world)
            : base(world)
        {
        }

        public override void DrawBelow(GameContext context)
        {
            context.Graphics.GraphicsDevice.Clear(Color.DarkGreen);
        }

        public override void DrawAbove(GameContext context)
        {
            //this.DrawDebugPathFindingGrid(context, new XnaGraphics(context));
        }

        public override void Update(GameContext context)
        {
        }
    }
}
