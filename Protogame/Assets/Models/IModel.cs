namespace Protogame
{
    using System;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// An interface representing a runtime model.
    /// </summary>
    public interface IModel
    {
        /// <summary>
        /// Gets the available animations.
        /// </summary>
        /// <value>
        /// The available animations.
        /// </value>
        IAnimationCollection AvailableAnimations { get; }

        /// <summary>
        /// Draws the model using the specified animation, calculating the appropriate frame to play
        /// based on how much time has elapsed.
        /// </summary>
        /// <param name="renderContext">
        /// The render context.
        /// </param>
        /// <param name="transform">
        /// The transform.
        /// </param>
        /// <param name="animationName">
        /// The animation name.
        /// </param>
        /// <param name="secondFraction">
        /// The seconds that have elapsed since the animation started playing.
        /// </param>
        /// <param name="multiply">
        /// The rate multiplier to apply.  A higher number multiplies the number of times the animation plays.
        /// </param>
        void Draw(IRenderContext renderContext, Matrix transform, string animationName, TimeSpan secondFraction, float multiply = 1);

        /// <summary>
        /// Draws the model using the specified animation, at a specific frame.
        /// </summary>
        /// <param name="renderContext">
        /// The render context.
        /// </param>
        /// <param name="transform">
        /// The transform.
        /// </param>
        /// <param name="animationName">
        /// The animation name.
        /// </param>
        /// <param name="frame">
        /// The frame.
        /// </param>
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