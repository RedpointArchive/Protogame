using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assimp;
using Microsoft.Xna.Framework.Graphics;

namespace Protogame
{
    public class Animation : IAnimation
    {
        public const string AnimationNullName = "(null)";

        public Animation(string name, IFrame[] frames, double ticksPerSecond, double durationInTicks)
        {
            this.DurationInTicks = durationInTicks;
            this.TicksPerSecond = ticksPerSecond;
            this.Frames = frames;
            this.Name = name;
        }

        public double DurationInTicks { get; private set; }

        public IFrame[] Frames { get; private set; }

        public string Name { get; private set; }

        public double TicksPerSecond
        {
            get;
            private set;
        }

        /// <summary>
        /// Loads vertex and index buffers for all of frames in this animation.
        /// </summary>
        /// <param name="graphicsDevice">
        /// The graphics device.
        /// </param>
        public void LoadBuffers(GraphicsDevice graphicsDevice)
        {
            foreach (var frame in Frames)
            {
                frame.LoadBuffers(graphicsDevice);
            }
        }
    }
}
