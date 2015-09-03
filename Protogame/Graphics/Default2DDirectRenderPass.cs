using System;
using Microsoft.Xna.Framework.Graphics;

namespace Protogame
{
    /// <summary>
    /// The default implementation of an <see cref="I2DDirectRenderPass"/>.
    /// </summary>
    /// <module>Graphics</module>
    /// <internal>True</internal>
    /// <interface_ref>Protogame.I2DDirectRenderPass</interface_ref>
    public class Default2DDirectRenderPass : I2DDirectRenderPass
    {
        /// <summary>
        /// Gets a value indicating that this is not a post-processing render pass.
        /// </summary>
        /// <value>Always false.</value>
        public bool IsPostProcessingPass
        {
            get
            {
                return false;
            }
        }

        public Viewport Viewport
        {
            get;
            set;
        }

        public void BeginRenderPass(IRenderContext renderContext, IRenderPass previousPass)
        {
        }

        public void EndRenderPass(IRenderContext renderContext, IRenderPass nextPass)
        {
        }
    }
}

