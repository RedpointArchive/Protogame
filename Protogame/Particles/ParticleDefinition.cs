using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Protogame.Particles
{
    public class ParticleDefinition
    {
        public string[] Images { get; set; }
        public float DirectionMin { get; set; }
        public float DirectionMax { get; set; }
        public float SpeedMin { get; set; }
        public float SpeedMax { get; set; }
        public float LifetimeMin { get; set; }
        public float LifetimeMax { get; set; }
        public ParticleMode RenderMode { get; set; }
        public Color Color { get; set; }

        public ParticleDefinition()
        {
            this.Color = Color.White;
        }
    }

    public enum ParticleMode
    {
        Background,
        Foreground
    }
}
