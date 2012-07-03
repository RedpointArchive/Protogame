using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Protogame;
using Protogame.MultiLevel;

namespace Example.RTS
{
    public class CaveLevel : PathFindingLevel
    {
        public CaveLevel(MultiLevelWorld world)
            : base(world)
        {
        }

        public override void DrawBelow(GameContext context)
        {
            context.Graphics.GraphicsDevice.Clear(Color.SaddleBrown);
        }

        public override void DrawAbove(GameContext context)
        {
        }

        public override void Update(GameContext context)
        {
        }
    }
}
