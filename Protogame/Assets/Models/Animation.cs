namespace Protogame
{
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// A default implementation of <see cref="IAnimation"/>.
    /// </summary>
    public class Animation : IAnimation
    {
        /// <summary>
        /// The name of the null animation, that is the animation of a mesh which describes
        /// no animation at all.
        /// </summary>
        public const string AnimationNullName = "(null)";

        /// <summary>
        /// Initializes a new instance of the <see cref="Animation"/> class.
        /// </summary>
        /// <param name="name">
        /// The name of the animation.
        /// </param>
        /// <param name="frames">
        /// The frames applicable to the animation.
        /// </param>
        /// <param name="ticksPerSecond">
        /// The ticks per second, or rate at which this animation plays.
        /// </param>
        /// <param name="durationInTicks">
        /// The duration in ticks, or total number of frames that this animation has.
        /// </param>
        public Animation(string name, IFrame[] frames, double ticksPerSecond, double durationInTicks)
        {
            this.DurationInTicks = durationInTicks;
            this.TicksPerSecond = ticksPerSecond;
            this.Frames = frames;
            this.Name = name;
        }

        /// <summary>
        /// Gets the duration in ticks.
        /// </summary>
        /// <value>
        /// The duration in ticks.
        /// </value>
        public double DurationInTicks { get; private set; }

        /// <summary>
        /// Gets the frames of the animation.
        /// </summary>
        /// <value>
        /// The frames of the animation.
        /// </value>
        public IFrame[] Frames { get; private set; }

        /// <summary>
        /// Gets the name of the animation.
        /// </summary>
        /// <value>
        /// The name of the animation.
        /// </value>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the ticks per second.
        /// </summary>
        /// <value>
        /// The ticks per second.
        /// </value>
        public double TicksPerSecond { get; private set; }

        /// <summary>
        /// Loads vertex and index buffers for all of frames in this animation.
        /// </summary>
        /// <param name="graphicsDevice">
        /// The graphics device.
        /// </param>
        public void LoadBuffers(GraphicsDevice graphicsDevice)
        {
            foreach (var frame in this.Frames)
            {
                frame.LoadBuffers(graphicsDevice);
            }
        }
    }
}