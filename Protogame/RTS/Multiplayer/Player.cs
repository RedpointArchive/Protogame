using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if MULTIPLAYER
using Process4.Attributes;
#endif

namespace Protogame.RTS.Multiplayer
{
    public static class StaticRandom
    {
        public static Random Generator = new Random();
    }

#if MULTIPLAYER
    [Distributed]
#endif
    public class Player
    {
#if !MULTIPLAYER
        public const int ID_NEUTRAL = 0;
        public const int ID_PLAYER = 1;
#endif

        public Player()
        {
            this.PlayerID = StaticRandom.Generator.Next(1, 65536);
        }

        public int PlayerID
        {
            get;
            private set;
        }

        public bool Ready
        {
            get;
            private set;
        }
        
#if MULTIPLAYER
        [ClientCallable]
#endif
        public void MarkReady(bool ready)
        {
            this.Ready = ready;
        }
    }
}
