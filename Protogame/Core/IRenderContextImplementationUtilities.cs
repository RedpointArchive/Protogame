using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Protogame
{
    /// <summary>
    /// An interface which provides utilities for implementing <see cref="IRenderContext"/>.
    /// <para>
    /// Internally, Protogame has two implementations of <see cref="IRenderContext"/>; one for
    /// the new render pipeline, and one for games written using the legacy graphics API.  This
    /// interface exists so that both of these implementations can share common code.
    /// </para>
    /// <para>
    /// You should never need to (though you can), use or implement this interface in your game code.
    /// </para>
    /// </summary>
    /// <module>Core API</module>
    public interface IRenderContextImplementationUtilities
    {
        /*
        /// <summary>
        /// Copies matrices from the outgoing effect to the incoming effect.  When
        /// you switch effects using <see cref="IRenderContext.PushEffect"/> or
        /// <see cref="IRenderContext.PopEffect"/>, the current world, view and
        /// projection matrices must be copied to the new effect by the engine.  Internally
        /// this is normally done by examining whether the outgoing and incoming effects
        /// have the <see cref="IWorldViewProjectionEffectSemantic"/> semantic.
        /// </summary>
        /// <param name="outgoing">The outgoing effect which is being removed from the stack.</param>
        /// <param name="incoming">The incoming effect which will be the effect used going forward.</param>
        void CopyMatricesToTargetEffect(IEffect outgoing, IEffect incoming);

        /// <summary>
        /// Returns the matrices from an effect.
        /// <para>
        /// Previously effects used to implement <see cref="IEffectMatrices"/> directly, but more modern effects
        /// implement <see cref="IEffectWithSemantics"/>, which has an <see cref="IWorldViewProjectionEffectSemantic"/>
        /// semantic (which implements <see cref="IEffectWithSemantics"/>).  This utility method first
        /// checks if the effect implements <see cref="IEffectMatrices"/> (and if it does, fetches the matrix from
        /// there), and falls back to retrieving the matrix using <see cref="IEffectWithSemantics"/> otherwise.
        /// </para>
        /// </summary>
        /// <param name="effect">The effect to retrieve matrices from.</param>
        /// <param name="prop">The matrix to retrieve.</param>
        /// <returns>The matrix value.</returns>
        Matrix GetEffectMatrix(IEffect effect, Func<IEffectMatrices, Matrix> prop);

        /// <summary>
        /// Sets the matrices from an effect.
        /// <para>
        /// Previously effects used to implement <see cref="IEffectMatrices"/> directly, but more modern effects
        /// implement <see cref="IEffectWithSemantics"/>, which has an <see cref="IWorldViewProjectionEffectSemantic"/>
        /// semantic (which implements <see cref="IEffectWithSemantics"/>).  This utility method first
        /// checks if the effect implements <see cref="IEffectMatrices"/> (and if it does, sets the matrix onto
        /// that implementation), and falls back to setting the matrix with <see cref="IEffectWithSemantics"/> otherwise.
        /// </para>
        /// </summary>
        /// <param name="effect">The effect to assign matrices to.</param>
        /// <param name="assign">The matrix to assign a value to.</param>
        void SetEffectMatrix(IEffect effect, Action<IEffectMatrices> assign);
        */
    }
}
