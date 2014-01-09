namespace Protogame
{
    using System;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// This represents a runtime model, with full support for animation and bone manipulation.
    /// </summary>
    public class Model : IModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Model"/> class.
        /// </summary>
        /// <param name="availableAnimations">
        /// The available animations.
        /// </param>
        public Model(IAnimationCollection availableAnimations)
        {
            this.AvailableAnimations = availableAnimations;
        }

        /// <summary>
        /// Gets the available animations.
        /// </summary>
        /// <value>
        /// The available animations.
        /// </value>
        public IAnimationCollection AvailableAnimations { get; private set; }

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
        public void Draw(IRenderContext renderContext, Matrix transform, string animationName, TimeSpan secondFraction)
        {
            // Normalize the animation name.
            if (string.IsNullOrEmpty(animationName))
            {
                animationName = Animation.AnimationNullName;
            }

            // Get a reference to the animation.
            var animation = this.AvailableAnimations[animationName];
            if (animation == null)
            {
                throw new InvalidOperationException("No such animation exists.");
            }

            // Multiply the total seconds by the ticks per second.
            var totalTicks = (int)(secondFraction.TotalSeconds * animation.TicksPerSecond);

            // Modulo the total ticks by the animation duration.
            var currentTick = (int)(totalTicks % animation.DurationInTicks);

            // Call the other draw function with the appropriate tick value.
            this.Draw(renderContext, transform, animationName, currentTick);
        }

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
        public void Draw(IRenderContext renderContext, Matrix transform, string animationName, int frame)
        {
            // Normalize the animation name.
            if (string.IsNullOrEmpty(animationName))
            {
                animationName = Animation.AnimationNullName;
            }

            var animation = this.AvailableAnimations[animationName];
            if (animation == null)
            {
                throw new InvalidOperationException("No such animation exists.");
            }

            // Get a reference to the frame we will render.
            var frames = animation.Frames;
            frame = frame % frames.Length;
            var frameRef = frames[frame];

            // Load the buffers for the frame if not already loaded (generally
            // developers are expected to call LoadBuffers on whatever animation
            // they will be playing).
            frameRef.LoadBuffers(renderContext.GraphicsDevice);

            // Keep a copy of the current world transformation and then apply the
            // transformation that was passed in.
            var oldWorld = renderContext.World;
            renderContext.World *= transform;

            // Render the vertex and index buffer.
            renderContext.GraphicsDevice.Indices = frameRef.IndexBuffer;
            renderContext.GraphicsDevice.SetVertexBuffer(frameRef.VertexBuffer);
            foreach (var pass in renderContext.Effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                renderContext.GraphicsDevice.DrawIndexedPrimitives(
                    PrimitiveType.TriangleList, 
                    0, 
                    0, 
                    frameRef.VertexBuffer.VertexCount, 
                    0, 
                    frameRef.IndexBuffer.IndexCount / 3);
            }

            // Restore the world matrix.
            renderContext.World = oldWorld;
        }

        /// <summary>
        /// Loads vertex and index buffers for all of animations in this model.
        /// </summary>
        /// <param name="graphicsDevice">
        /// The graphics device.
        /// </param>
        public void LoadBuffers(GraphicsDevice graphicsDevice)
        {
            foreach (var animation in this.AvailableAnimations)
            {
                animation.LoadBuffers(graphicsDevice);
            }
        }
    }
}