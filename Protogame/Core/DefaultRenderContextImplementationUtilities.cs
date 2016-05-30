using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Protogame
{
    public class DefaultRenderContextImplementationUtilities : IRenderContextImplementationUtilities
    {
        public void CopyMatricesToTargetEffect(Effect outgoing, Effect incoming)
        {
            Matrix? projection = null, view = null, world = null;
            if (outgoing is IEffectMatrices)
            {
                var outgoingMatrices = (IEffectMatrices)outgoing;
                projection = outgoingMatrices.Projection;
                view = outgoingMatrices.View;
                world = outgoingMatrices.World;
            }
            else if (outgoing is IEffectWithSemantics)
            {
                var outgoingSemantics = (IEffectWithSemantics)outgoing;
                if (outgoingSemantics.HasSemantic<IWorldViewProjectionEffectSemantic>())
                {
                    var outgoingMatrices = outgoingSemantics.GetSemantic<IWorldViewProjectionEffectSemantic>();
                    projection = outgoingMatrices.Projection;
                    view = outgoingMatrices.View;
                    world = outgoingMatrices.World;
                }
            }

            if (projection != null && view != null && world != null)
            {
                if (incoming is IEffectMatrices)
                {
                    var incomingMatrices = (IEffectMatrices)incoming;
                    incomingMatrices.Projection = projection.Value;
                    incomingMatrices.View = view.Value;
                    incomingMatrices.World = world.Value;
                }
                else if (incoming is IEffectWithSemantics)
                {
                    var incomingSemantics = (IEffectWithSemantics)incoming;
                    if (incomingSemantics.HasSemantic<IWorldViewProjectionEffectSemantic>())
                    {
                        var incomingMatrices = incomingSemantics.GetSemantic<IWorldViewProjectionEffectSemantic>();
                        incomingMatrices.Projection = projection.Value;
                        incomingMatrices.View = view.Value;
                        incomingMatrices.World = world.Value;
                    }
                }
            }
        }

        public Matrix GetEffectMatrix(Effect effect, Func<IEffectMatrices, Matrix> prop)
        {
            var effectMatrices = effect as IEffectMatrices;

            if (effectMatrices != null)
            {
                return prop(effectMatrices);
            }

            var effectSemantic = effect as IEffectWithSemantics;
            if (effectSemantic != null)
            {
                if (effectSemantic.HasSemantic<IWorldViewProjectionEffectSemantic>())
                {
                    return prop(effectSemantic.GetSemantic<IWorldViewProjectionEffectSemantic>());
                }
            }

            return Matrix.Identity;
        }

        public void SetEffectMatrix(Effect effect, Action<IEffectMatrices> assign)
        {
            var effectMatrices = effect as IEffectMatrices;

            if (effectMatrices != null)
            {
                assign(effectMatrices);
            }

            var effectSemantic = effect as IEffectWithSemantics;
            if (effectSemantic != null)
            {
                if (effectSemantic.HasSemantic<IWorldViewProjectionEffectSemantic>())
                {
                    assign(effectSemantic.GetSemantic<IWorldViewProjectionEffectSemantic>());
                }
            }
        }
    }
}