using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;

namespace Protogame
{
    public class EffectWithSemantics : Effect, IEffectWithSemantics
    {
        public IEffectSemantic[] EffectSemantics { get; }

        public EffectWithSemantics(GraphicsDevice device, byte[] bytecode, IEffectSemantic[] availableSemantics)
            : base(device, bytecode)
        {
            var semantics = new List<IEffectSemantic>();
            foreach (var s in availableSemantics)
            {
                if (s.ShouldAttachToEffect(this))
                {
                    s.AttachToEffect(this);
                    semantics.Add(s);
                }
            }

            this.EffectSemantics = semantics.ToArray();

            this.CacheEffectParameters();
        }

        public EffectWithSemantics(GraphicsDevice device, IEffectReader effectReader, IEffectSemantic[] availableSemantics)
            : base(device, effectReader)
        {
            var semantics = new List<IEffectSemantic>();
            foreach (var s in availableSemantics)
            {
                if (s.ShouldAttachToEffect(this))
                {
                    s.AttachToEffect(this);
                    semantics.Add(s);
                }
            }

            this.EffectSemantics = semantics.ToArray();

            this.CacheEffectParameters();
        }

        protected EffectWithSemantics(EffectWithSemantics cloneSource) : base(cloneSource)
        {
            this.EffectSemantics =
                cloneSource.EffectSemantics.Select(x => x.Clone(this)).ToArray();

            this.CacheEffectParameters();
        }

        public bool HasSemantic<T>() where T : IEffectSemantic
        {
            return this.EffectSemantics.OfType<T>().Any();
        }

        public T GetSemantic<T>() where T : IEffectSemantic
        {
            return this.EffectSemantics.OfType<T>().First();
        }

        protected override bool OnApply()
        {
            foreach (var semantic in this.EffectSemantics)
            {
                semantic.OnApply();
            }

            return false;
        }

        private void CacheEffectParameters()
        {
            foreach (var semantic in this.EffectSemantics)
            {
                semantic.CacheEffectParameters();
            }
        }
    }
}
