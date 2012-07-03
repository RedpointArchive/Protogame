using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Protogame;

namespace Protogame.MultiLevel
{
    public class MultiLevelEntityTile : EntityTile, IMultiLevelEntity
    {
        public Level Level
        {
            get;
            set;
        }
    }
}
