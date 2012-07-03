using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if MULTIPLAYER
using Process4.Attributes;
using Process4.Interfaces;
#endif

namespace Protogame.RTS.Multiplayer
{
#if MULTIPLAYER
    [Serializable]
#endif
    public class MoveEventArgs : EventArgs
    {
        public float X;
        public float Y;
    }

#if MULTIPLAYER
    [Serializable]
#endif
    public class AttackEventArgs : EventArgs
    {
        public string UnitNetworkName;
    }
    
#if MULTIPLAYER
    [Distributed]
#endif
    public class UnitSynchronisationData
    {
        public event EventHandler Move;
        public event EventHandler Attack;
        
#if MULTIPLAYER
        [ClientCallable]
        public void BroadcastMove(float x, float y)
        {
            if (this.Move != null)
                this.Move(this, new MoveEventArgs() { X = x, Y = y });
        }

        [ClientCallable]
        public void BroadcastAttack(UnitSynchronisationData udata)
        {
            if (this.Attack != null)
                this.Attack(this, new AttackEventArgs() { UnitNetworkName = (udata as ITransparent).NetworkName });
        }
#else
        public void BroadcastMove(float x, float y)
        {
        }

        public void BroadcastAttack(UnitSynchronisationData udata)
        {
        }
#endif
    }
}
