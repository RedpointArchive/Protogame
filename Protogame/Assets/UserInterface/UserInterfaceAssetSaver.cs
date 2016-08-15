namespace Protogame
{
    public class UserInterfaceAssetSaver : IAssetSaver
    {
        public bool CanHandle(IAsset asset)
        {
            return asset is UserInterfaceAsset;
        }
        
        public IRawAsset Handle(IAsset asset, AssetTarget target)
        {
            var userInterfaceAsset = asset as UserInterfaceAsset;

            return
                new AnonymousObjectBasedRawAsset(
                    new
                    {
                        Loader = typeof(UserInterfaceAssetLoader).FullName,
                        userInterfaceAsset.UserInterfaceData,
                        userInterfaceAsset.UserInterfaceFormat,
                        userInterfaceAsset.SourcePath
                    });
        }
    }
}