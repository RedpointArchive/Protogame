using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Protogame;
using Microsoft.Xna.Framework;
using Protogame.MultiLevel;

namespace Example.RTS
{
    public class PocketLevel : PathFindingLevel
    {
        public PocketLevel(MultiLevelWorld world)
            : base(world)
        {
        }

        public override void DrawBelow(GameContext context)
        {
            context.Graphics.GraphicsDevice.Clear(Color.DarkSlateGray);
        }

        public override void DrawAbove(GameContext context)
        {
        }

        public override void Update(GameContext context)
        {
        }
    }
}
