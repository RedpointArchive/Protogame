using System;

namespace Protogame
{
    /// <summary>
    /// The interface for the rendering pipeline.
    /// </summary>
    /// <module>Graphics</module>
    public interface IRenderPipeline
    {
        /// <summary>
        /// Renders the game using the render pipeline.
        /// </summary>
        /// <param name="gameContext">The current game context.</param>
        /// <param name="renderContext">The current render context.</param>
        void Render(IGameContext gameContext, IRenderContext renderContext);

        /// <summary>
        /// Adds the specified render pass to the render pipeline
        /// permanently.  This render pass will take effect after the
        /// start of the next frame.
        /// </summary>
        /// <returns>
        /// The render pass that was given to this function.  This return
        /// value is for convenience only, so that you may construct and add
        /// a render pass in a single statement, while obtaining a reference to
        /// it if you need to modify it's values or call <see cref="RemoveFixedRenderPass"/>
        /// later.  The render pass is not modified by this function.
        /// </returns>
        /// <param name="renderPass">The render pass to add.</param>
        IRenderPass AddFixedRenderPass(IRenderPass renderPass);

        /// <summary>
        /// Removes the specified render pass from the render pipeline.
        /// </summary>
        /// <param name="renderPass">The render pass to remove.</param>
        void RemoveFixedRenderPass(IRenderPass renderPass);

        /// <summary>
        /// Append the specified render pass to the render pipeline
        /// for this frame only.  This is method that allows you to temporarily
        /// add additional render passes to a frame.
        /// <para>
        /// If all standard (non-post-processing) render passes have finished
        /// post-processing has begun and this method is given a standard render
        /// pass, it will have no effect.
        /// </para>
        /// <para>
        /// Render passes that were appended can not be removed with
        /// <see cref="RemoveFixedRenderPass"/>.
        /// </para>
        /// </summary>
        /// <returns>
        /// The render pass that was given to this function.  This return
        /// value is for convenience only, so that you may construct and add
        /// a render pass in a single statement, while obtaining a reference to
        /// it if you need to modify it's value.  The render pass is not
        /// modified by this function.
        /// </returns>
        /// <param name="renderPass">The render pass to add.</param>
        IRenderPass AppendTransientRenderPass(IRenderPass renderPass);

        /// <summary>
        /// Returns the current render pass.  Returns null if the code is
        /// not currently executing from within <see cref="Render"/>.
        /// </summary>
        /// <returns>The current render pass, or null if the render pipeline isn't rendering.</returns>
        IRenderPass GetCurrentRenderPass();

        /// <summary>
        /// Returns if the current render pass is the first one in the pipeline.
        /// </summary>
        /// <returns>Whether the current render pass is the first one in the pipeline.</returns>
        bool IsFirstRenderPass();
    }
}

