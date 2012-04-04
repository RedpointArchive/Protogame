using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Protogame;
using Protogame.Platformer;
using Protogame.Particles;

namespace Protogame.Platformer
{
    public abstract class Player : GravityEntity
    {
        public virtual int PlayerYOffset { get { return 0; } }
        public virtual float PlayerMovementSpeed { get { return 4; } }
        public virtual float PlayerJumpSpeed { get { return 5; } }
        private bool m_PlayerIsMoving = false;

        protected Player()
        {
        }

        public bool CollidesWithSolidAt(World world, int x, int y)
        {
            return Helpers.CollidesWithSolidAt(this, world, x, y);
        }

        public virtual void MoveLeft(World world)
        {
            this.m_PlayerIsMoving = true;
            this.ImageFlipX = true;
            if (this.CollidesWithSolidAt(world, (int)(this.X - this.PlayerMovementSpeed), (int)this.Y - 1))
                this.MoveUntilContact(world, -1, 0, this.PlayerMovementSpeed);
            else
                this.X -= this.PlayerMovementSpeed;
        }

        public virtual void MoveRight(World world)
        {
            this.m_PlayerIsMoving = true;
            this.ImageFlipX = false;
            if (this.CollidesWithSolidAt(world, (int)(this.X + this.PlayerMovementSpeed), (int)this.Y - 1))
                this.MoveUntilContact(world, 1, 0, this.PlayerMovementSpeed);
            else
                this.X += this.PlayerMovementSpeed;
        }

        public virtual void MoveEnd()
        {
            this.m_PlayerIsMoving = false;
        }

        public virtual void Jump(World world)
        {
            if (this.TouchingGroundLastStep && !this.CollidesWithSolidAt(world, (int)this.X, (int)this.Y - 16))
                this.YSpeed = -this.PlayerJumpSpeed;
        }

        public virtual void JumpEnd()
        {
            if (!this.TouchingGroundLastStep && this.YSpeed < 0)
                this.YSpeed /= 1.5f;
        }

        public virtual void Action(World world)
        {
        }

        public override void Update(World world)
        {
            base.Update(world);

            // Movement handling.
            this.XSpeed = 0;

            // Gravity.
            if (this.YSpeed > 12) this.YSpeed = 12;

            // Death.
            if (this.Y > Tileset.TILESET_PIXEL_HEIGHT)
                world.LoadLevel(world.CurrentLevel);

            if (!this.m_PlayerIsMoving)
                this.PerformAlignment(8, 4, () => { this.MoveLeft(world); }, () => { this.MoveRight(world); });
        }
    }
}
