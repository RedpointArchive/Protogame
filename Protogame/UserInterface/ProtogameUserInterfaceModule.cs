using Protoinject;

namespace Protogame
{
    public class ProtogameUserInterfaceModule: IProtoinjectModule
    {
        public void Load(IKernel kernel)
        {
        }
        
        public static ISkin GetDefaultSkin(IKernel kernel)
        {
            return new BasicSkin(
                new DefaultBasicSkin(),
                kernel.Get<I2DRenderUtilities>(),
                kernel.Get<IAssetManagerProvider>());
        }
    }
}
