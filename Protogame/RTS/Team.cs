using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Protogame.RTS.Multiplayer;
#if MULTIPLAYER
using Process4.Collections;
#endif

namespace Protogame.RTS
{
    public class Team
    {
        public Team(int id, string name, Color color)
        {
            this.Name = name;
            this.Color = color;
#if MULTIPLAYER
            this.SynchronisationData = new Distributed<TeamSynchronisationData>("rts-team-" + id);
#else
            this.SynchronisationData = new TeamSynchronisationData();
#endif
            this.SynchronisationData.SetPlayerID(id);
        }

        public string Name
        {
            get;
            private set;
        }

        public Color Color
        {
            get;
            private set;
        }

        public TeamSynchronisationData SynchronisationData
        {
            get;
            private set;
        }
    }
}
