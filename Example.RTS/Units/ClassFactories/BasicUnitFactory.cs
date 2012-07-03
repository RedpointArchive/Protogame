using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Protogame.RTS;
using Protogame.MultiLevel;

namespace Example.RTS.Units.ClassFactories
{
    public class BasicUnitFactory : IUnitFactory
    {
        #region IUnitFactory Members

        public bool CanCreate(string name)
        {
            return name == "BasicUnit";
        }

        public Unit Create(RTSWorld world, Level level, Team team, string name, Dictionary<string, string> attributes)
        {
            switch (name)
            {
                case "BasicUnit":
                    BasicUnit b = new BasicUnit(level);
                    b.Team = team;
                    return b;
            }

            throw new NotSupportedException();
        }

        #endregion
    }
}
