using Microsoft.Xna.Framework;

#if LEGACY

namespace Protogame
{
    public class ParticleDefinition
    {
        public TextureAsset[] Images { get; set; }
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

#endif