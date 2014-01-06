using Microsoft.Xna.Framework.Graphics;

namespace Protogame
{
    /// <summary>
    /// An interface representing a model's animation that can be played back at runtime.
    /// </summary>
    public interface IAnimation
    {
        /// <summary>
        /// Gets the duration in ticks.
        /// </summary>
        /// <value>
        /// The duration in ticks.
        /// </value>
        double DurationInTicks { get; }

        /// <summary>
        /// Gets the frames for this animation.
        /// </summary>
        /// <value>
        /// The frames for this animation.
        /// </value>
        IFrame[] Frames { get; }

        /// <summary>
        /// Gets the name of the animation.
        /// </summary>
        /// <value>
        /// The name of the animation.
        /// </value>
        string Name { get; }

        /// <summary>
        /// Gets the ticks per second.
        /// </summary>
        /// <value>
        /// The ticks per second.
        /// </value>
        double TicksPerSecond
        {
            get;
        }

        /// <summary>
        /// Loads vertex and index buffers for all of frames in this animation.
        /// </summary>
        /// <param name="graphicsDevice">
        /// The graphics device.
        /// </param>
        void LoadBuffers(GraphicsDevice graphicsDevice);
    }
}