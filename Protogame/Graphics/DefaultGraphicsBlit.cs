using System;
using Microsoft.Xna.Framework.Graphics;

namespace Protogame
{
    /// <summary>
    /// A default implementation of <see cref="IGraphicsBlit"/>.
    /// </summary>
    /// <module>Graphics</module>
    /// <internal>True</internal>
    /// <interface_ref>Protogame.IGraphicsBlit</interface_ref>
    public class DefaultGraphicsBlit : IGraphicsBlit
    {
        private readonly Effect _blitEffect;

        public DefaultGraphicsBlit(IAssetManagerProvider assetManagerProvider)
        {
            _blitEffect = assetManagerProvider.GetAssetManager().Get<EffectAsset>("effect.Blit").Effect;
        }

        public void Blit(RenderTarget2D source, RenderTarget2D destination = null, Effect shader = null)
        {
            throw new NotImplementedException();
        }
    }
}
