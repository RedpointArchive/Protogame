using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Protogame.RTS;
using Protogame.MultiLevel;

namespace Example.RTS.Units
{
    public class BasicUnit : MovableUnit
    {
        public BasicUnit(Level initial)
            : base(initial)
        {
            this.Width = 16;
            this.Height = 16;
            this.Images = this.GetTexture("units.infantry.soldier");
            this.ImageSpeed = 1;
            
            // RTS unit attributes.
            this.MaxHealth = 100;
            this.MoveSpeed = 3;
            this.AttackPower = 2;
            this.AttackRange = 48; // pixels
        }
    }
}
