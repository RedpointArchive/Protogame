using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Protogame;

namespace Protogame.MultiLevel
{
    public class MultiLevelEntity : Entity, IMultiLevelEntity
    {
        public MultiLevelEntity(Level level)
            : base()
        {
            this.Level = level;
        }

        public Level Level
        {
            get;
            set;
        }
    }
}
