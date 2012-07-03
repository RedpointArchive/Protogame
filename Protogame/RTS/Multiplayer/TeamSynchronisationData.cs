using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if MULTIPLAYER
using Process4.Attributes;
#endif

namespace Protogame.RTS.Multiplayer
{
#if MULTIPLAYER
    [Distributed]
#endif
    public class TeamSynchronisationData
    {
        public int PlayerID { get; private set; }
        
#if MULTIPLAYER
        [ClientCallable]
#endif
        public void SetPlayerID(int id)
        {
            this.PlayerID = id;
        }
    }
}
