using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Protogame;
using Protogame.Platformer;
using Protogame.Particles;

namespace Example.Entities
{
    public class Player : Protogame.Platformer.Player
    {
        // Settings
        public override int PlayerYOffset { get { return 0; } }
        public override float PlayerMovementSpeed { get { return 4; } }
        public override float PlayerJumpSpeed { get { return 5; } }

        public Player()
        {
            this.Images = this.GetTextureAnim("player.frame", 5);
            this.Width = 32;
            this.Height = 32;
            this.ImageSpeed = 5;
        }
    }
}
