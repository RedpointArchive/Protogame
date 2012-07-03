using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Protogame.MultiLevel;

namespace Protogame.RTS
{
    public interface IUnitFactory
    {
        bool CanCreate(string name);
        Unit Create(RTSWorld world, Level level, Team team, string name, Dictionary<string, string> attributes);
    }
}
