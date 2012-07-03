using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Protogame;

namespace Protogame.MultiLevel
{
    public abstract class Level
    {
        public Level(MultiLevelWorld world)
        {
            this.Entities = new List<IMultiLevelEntity>();
            this.World = world;
        }

        public virtual List<IMultiLevelEntity> Entities
        {
            get;
            private set;
        }

        public MultiLevelWorld World
        {
            get;
            private set;
        }

        public abstract void DrawBelow(GameContext context);
        public abstract void DrawAbove(GameContext context);
        public abstract void Update(GameContext context);
    }
}
