using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Protogame
{
    public interface IRenderContextImplementationUtilities
    {
        void CopyMatricesToTargetEffect(Effect outgoing, Effect incoming);
        Matrix GetEffectMatrix(Effect effect, Func<IEffectMatrices, Matrix> prop);
        void SetEffectMatrix(Effect effect, Action<IEffectMatrices> assign);
    }
}
