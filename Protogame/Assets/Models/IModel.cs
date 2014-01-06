using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Protogame
{
    /// <summary>
    /// An interface representing a runtime model.
    /// </summary>
    public interface IModel
    {
        IAnimationCollection AvailableAnimations { get; }

        void Draw(IRenderContext renderContext, Matrix transform, string animationName, TimeSpan secondFraction);

        void Draw(IRenderContext renderContext, Matrix transform, string animationName, int frame);

        /// <summary>
        /// Loads vertex and index buffers for all of animations in this model.
        /// </summary>
        /// <param name="graphicsDevice">
        /// The graphics device.
        /// </param>
        void LoadBuffers(GraphicsDevice graphicsDevice);
    }
}
