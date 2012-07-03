using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Protogame;
using Protogame.MultiLevel;

namespace Example.RTS
{
    public class SmallMilitaryLevel : PathFindingLevel
    {
        public SmallMilitaryLevel(MultiLevelWorld world)
            : base(world)
        {
        }

        public override void DrawBelow(GameContext context)
        {
            context.Graphics.GraphicsDevice.Clear(Color.Gray);
        }

        public override void DrawAbove(GameContext context)
        {
        }

        public override void Update(GameContext context)
        {
        }
    }
}
