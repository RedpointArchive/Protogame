#if PLATFORM_ANDROID

namespace Protogame
{
    public class PlatformAndroidAssetFsLayer : AndroidAssetFsLayer
    {
        public PlatformAndroidAssetFsLayer() : base(TargetPlatformUtility.GetExecutingPlatform().ToString())
        {
        }
    }
}

#endif