namespace Protogame
{
    public class UserInterfaceAssetLoader : IAssetLoader
    {
        public bool CanHandle(IRawAsset data)
        {
            return data.GetProperty<string>("Loader") == typeof(UserInterfaceAssetLoader).FullName;
        }
        
        public bool CanNew()
        {
            return true;
        }
        
        public IAsset GetDefault(IAssetManager assetManager, string name)
        {
            return null;
        }

        public IAsset GetNew(IAssetManager assetManager, string name)
        {
            return new UserInterfaceAsset(name, null, UserInterfaceFormat.Unknown, string.Empty);
        }
        
        public IAsset Handle(IAssetManager assetManager, string name, IRawAsset data)
        {
            return new UserInterfaceAsset(
                name,
                data.GetProperty<string>("UserInterfaceData"),
                data.GetProperty<UserInterfaceFormat>("UserInterfaceFormat"),
                data.GetProperty<string>("SourcePath"));
        }
    }
}