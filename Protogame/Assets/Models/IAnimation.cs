namespace Protogame
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;

    /// <summary>
    /// An interface representing a model's animation that can be played back at runtime.
    /// </summary>
    public interface IAnimation
    {
        /// <summary>
        /// Gets the duration of the animation in ticks.
        /// </summary>
        /// <remarks>
        /// This value multiplied by <see cref="TicksPerSecond"/> will give you the
        /// total number of seconds that the animation runs for.
        /// </remarks>
        /// <value>
        /// The duration of the animation in ticks.
        /// </value>
        double DurationInTicks { get; }

        /// <summary>
        /// Gets the name of the animation.
        /// </summary>
        /// <value>
        /// The name of the animation.
        /// </value>
        string Name { get; }

        /// <summary>
        /// Gets a read-only representation of the rotation keys used in this animation.
        /// </summary>
        /// <remarks>
        /// Modifying this dictionary will not have the intended effect.
        /// </remarks>
        /// <value>
        /// A read-only representation of the rotation keys used in this animation.
        /// </value>
        IDictionary<string, IDictionary<double, Quaternion>> RotationKeys { get; }

        /// <summary>
        /// Gets a read-only representation of the scale keys used in this animation.
        /// </summary>
        /// <remarks>
        /// Modifying this dictionary will not have the intended effect.
        /// </remarks>
        /// <value>
        /// A read-only representation of the scale keys used in this animation.
        /// </value>
        IDictionary<string, IDictionary<double, Vector3>> ScaleKeys { get; }

        /// <summary>
        /// Gets the number of ticks that occur per second for this animation.
        /// </summary>
        /// <value>
        /// The number of ticks that occur per second for this animation.
        /// </value>
        double TicksPerSecond { get; }

        /// <summary>
        /// Gets a read-only representation of the translation keys used in this animation.
        /// </summary>
        /// <remarks>
        /// Modifying this dictionary will not have the intended effect.
        /// </remarks>
        /// <value>
        /// A read-only representation of the translation keys used in this animation.
        /// </value>
        IDictionary<string, IDictionary<double, Vector3>> TranslationKeys { get; }

        /// <summary>
        /// Applies the animation at a specified time to the model.
        /// </summary>
        /// <param name="model">
        /// The model to apply the animation to.
        /// </param>
        /// <param name="frame">
        /// The frame to draw at.
        /// </param>
        void Apply(Model model, double frame);

        /// <summary>
        /// Applies the animation at a specified time to the model.
        /// </summary>
        /// <param name="model">
        /// The model to apply the animation to.
        /// </param>
        /// <param name="totalSeconds">
        /// The time elapsed.
        /// </param>
        /// <param name="multiply">
        /// The multiplication factor to apply to the animation speed.
        /// </param>
        void Apply(Model model, float totalSeconds, float multiply);

        /// <summary>
        /// Modifies the specified model to align to this animation at the specified frame and then renders it.
        /// </summary>
        /// <param name="renderContext">
        /// The current render context.
        /// </param>
        /// <param name="transform">
        /// The world transformation to apply.
        /// </param>
        /// <param name="model">
        /// The model to update.
        /// </param>
        /// <param name="secondFraction">
        /// The time elapsed.
        /// </param>
        /// <param name="multiply">
        /// The multiplication factor to apply to the animation speed.
        /// </param>
        void Render(
            IRenderContext renderContext, 
            Matrix transform, 
            Model model, 
            TimeSpan secondFraction, 
            float multiply);

        /// <summary>
        /// Modifies the specified model to align to this animation at the specified frame and then renders it.
        /// </summary>
        /// <param name="renderContext">
        /// The current render context.
        /// </param>
        /// <param name="transform">
        /// The world transformation to apply.
        /// </param>
        /// <param name="model">
        /// The model to update.
        /// </param>
        /// <param name="totalSeconds">
        /// The time elapsed.
        /// </param>
        /// <param name="multiply">
        /// The multiplication factor to apply to the animation speed.
        /// </param>
        void Render(IRenderContext renderContext, Matrix transform, Model model, float totalSeconds, float multiply);

        /// <summary>
        /// Modifies the specified model to align to this animation at the specified frame and then renders it.
        /// </summary>
        /// <param name="renderContext">
        /// The current render context.
        /// </param>
        /// <param name="transform">
        /// The world transformation to apply.
        /// </param>
        /// <param name="model">
        /// The model to update.
        /// </param>
        /// <param name="frame">
        /// The frame to draw at.
        /// </param>
        void Render(IRenderContext renderContext, Matrix transform, Model model, double frame);
    }
}