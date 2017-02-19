namespace Protogame
{
    public class UserInterfaceAssetLoader : IAssetLoader
    {
        public bool CanLoad(IRawAsset data)
        {
            return data.GetProperty<string>("Loader") == typeof(UserInterfaceAssetLoader).FullName;
        }

        public IAsset Load(string name, IRawAsset data)
        {
            return new UserInterfaceAsset(
                name,
                data.GetProperty<string>("UserInterfaceData"),
                data.GetProperty<UserInterfaceFormat>("UserInterfaceFormat"),
                data.GetProperty<string>("SourcePath"));
        }
    }
}