using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Protogame
{
    /// <summary>
    /// The default implementation of an <see cref="I3DRenderPass"/>.
    /// </summary>
    /// <module>Graphics</module>
    /// <internal>True</internal>
    /// <interface_ref>Protogame.I3DRenderPass</interface_ref>
    public class Default3DRenderPass : I3DRenderPass
    {
        public Viewport Viewport { get; set; }

        public Matrix? Projection { get; set; }

        public Matrix? View { get; set; }

        /// <summary>
        /// Gets a value indicating that this is not a post-processing render pass.
        /// </summary>
        /// <value>Always false.</value>
        public bool IsPostProcessingPass
        {
            get { return false; }
        }

        public void BeginRenderPass(IGameContext gameContext, IRenderContext renderContext, IRenderPass previousPass, RenderTarget2D postProcessingSource)
        {
        }

        public void EndRenderPass(IGameContext gameContext, IRenderContext renderContext, IRenderPass nextPass)
        {
        }
    }
}
