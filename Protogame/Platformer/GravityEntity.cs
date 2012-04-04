using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace Protogame.Platformer
{
    public class GravityEntity : Entity
    {
        public const float Gravity = 0.175f;
        public bool TouchingGroundLastStep { get; set; }
        private int m_GroundTouchTimer = 0;
        public bool Collidable { get; set; }

        public GravityEntity()
        {
            this.Collidable = true;
        }

        protected void MoveUntilContact(World world, float x, float y, double max)
        {
            for (int i = 0; i < Math.Abs(max); i += 1)
            {
                // Attempt adjustment.
                this.X += x;
                this.Y += y;

                // Check with bounding boxes of solid tiles.
                ReadOnlyCollection<Tile> linear = world.Tileset.AsLinear();
                foreach (Tile t in linear)
                {
                    if (t is ISolid)
                        if (BoundingBox.Check(this, t))
                        {
                            // Undo the last state and return.
                            this.X -= x;
                            this.Y -= y;
                            return;
                        }
                }
                foreach (IEntity e in world.Entities)
                {
                    if (e == this) continue;
                    if (e is ISolid)
                        if (BoundingBox.Check(this, e as IBoundingBox))
                        {
                            // Undo the last state and return.
                            this.X -= x;
                            this.Y -= y;
                            return;
                        }
                }
            }
        }

        public override void Update(World world)
        {
            base.Update(world);

            // Update gravity and positions.
            if (this.m_GroundTouchTimer == 0)
                this.TouchingGroundLastStep = false;
            else
                this.m_GroundTouchTimer -= 1;
            this.YSpeed += GravityEntity.Gravity;
            this.X += this.XSpeed;
            this.Y += this.YSpeed;

            if (this.Collidable)
            {
                // Check with bounding boxes of solid tiles.
                ReadOnlyCollection<Tile> linear = world.Tileset.AsLinear();
                foreach (Tile t in linear)
                {
                    if (t is ISolid)
                        if (BoundingBox.Check(this, t))
                        {
                            // Undo last whole step.
                            this.X -= this.XSpeed;
                            this.Y -= this.YSpeed;

                            // Update X / Y positions.
                            this.MoveUntilContact(world, this.X / Math.Abs(this.X), 0, this.XSpeed);
                            this.MoveUntilContact(world, 0, this.Y / Math.Abs(this.Y), this.YSpeed);

                            // Set Y-speed to 0.
                            this.YSpeed = 0;

                            // Set touching ground to true.
                            if (t.Y > this.Y)
                            {
                                this.TouchingGroundLastStep = true;
                                this.m_GroundTouchTimer = 3;
                            }
                        }
                }

                // Check with bounding boxes of solid entities.
                foreach (IEntity e in world.Entities)
                {
                    if (e == this) continue;
                    if (e is ISolid)
                        if (BoundingBox.Check(this, e as IBoundingBox))
                        {
                            // Undo last whole step.
                            this.X -= this.XSpeed;
                            this.Y -= this.YSpeed;

                            // Update X / Y positions.
                            this.MoveUntilContact(world, this.X / Math.Abs(this.X), 0, this.XSpeed);
                            this.MoveUntilContact(world, 0, this.Y / Math.Abs(this.Y), this.YSpeed);

                            // Set Y-speed to 0.
                            this.YSpeed = 0;

                            // Set touching ground to true.
                            if (e.Y > this.Y)
                            {
                                this.TouchingGroundLastStep = true;
                                this.m_GroundTouchTimer = 3;
                            }
                        }
                }
            }
        }
    }
}
