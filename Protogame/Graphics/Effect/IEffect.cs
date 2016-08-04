using Microsoft.Xna.Framework.Graphics;

namespace Protogame
{
    public interface IEffect
    {
        string Name { get; }

        Effect NativeEffect { get; }

        IEffectParameterCollection Parameters { get; }

        IEffectTechniqueCollection Techniques { get; }

        IEffectTechnique CurrentTechnique { get; set; }

        IEffectParameterSet CreateParameterSet();

        void LoadParameterSet(IRenderContext renderContext, IEffectParameterSet effectParameters, bool skipMatricSync = false);
    }
}
