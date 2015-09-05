using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Protogame
{
    /// <summary>
    /// A render pass in which the view and projection matrixes
    /// are configured if set in the render pass' settings.
    /// </summary>
    public interface I3DRenderPass : IRenderPass
    {
        /// <summary>
        /// Gets or sets the viewport used in this rendering pass.
        /// <para>
        /// By configuring different viewports on multiple render passes, you
        /// can easily configure split-screen games, where different viewports
        /// are used for different players.
        /// </para>
        /// </summary>
        /// <value>The viewport used for rendering.</value>
        Viewport Viewport { get; set; }

        /// <summary>
        /// Gets or sets the projection matrix to use in this rendering
        /// pass.  If this value is set to null, then the projection matrix
        /// is not modified when starting this render pass.
        /// </summary>
        /// <value>The projection matrix to use, or null to not modify.</value>
        Matrix? Projection { get; set; }

        /// <summary>
        /// Gets or sets the view matrix to use in this rendering
        /// pass.  If this value is set to null, then the view matrix
        /// is not modified when starting this render pass.
        /// </summary>
        /// <value>The view matrix to use, or null to not modify.</value>
        Matrix? View { get; set; }
    }
}
