using System;

namespace Protogame
{
    public class ParticleEntity : Entity
    {
        public static Random Random = new Random();

        public ParticleDefinition Definition { get; private set; }
        public float Direction { get; private set; }
        public float Speed { get; private set; }
        public float Lifetime { get; private set; }
        public float Tick { get; private set; }

        public ParticleEntity(ParticleDefinition definition)
        {
            this.Definition = definition;
            this.Tick = 0;
            this.Width = 4;
            this.Height = 4;

            // Determine particle values.
            this.Images = this.Definition.Images;
            this.Color = this.Definition.Color;
            this.Direction = (float)Random.NextDouble() * (this.Definition.DirectionMax - this.Definition.DirectionMin) + this.Definition.DirectionMin;
            this.Speed = (float)Random.NextDouble() * (this.Definition.SpeedMax - this.Definition.SpeedMin) + this.Definition.SpeedMin;
            this.Lifetime = (float)Random.NextDouble() * (this.Definition.LifetimeMax - this.Definition.LifetimeMin) + this.Definition.LifetimeMin;
        }

        public override void Update(IUpdateContext context)
        {
            base.Update(context);

            // Check to see if this particle should be removed.
            if (this.Tick >= this.Lifetime)
            {
                // FIXME
                //world.Entities.Remove(this);
                return;
            }

            this.X += (float)Math.Cos((double)this.Direction) * this.Speed;
            this.Y += (float)Math.Sin((double)this.Direction) * this.Speed;
            this.Tick += 1;
        }
    }
}
