namespace Protogame
{
    public class LevelAssetLoader : IAssetLoader
    {
        public bool CanLoad(IRawAsset data)
        {
            return data.GetProperty<string>("Loader") == typeof(LevelAssetLoader).FullName;
        }
        
        public IAsset Load(string name, IRawAsset data)
        {
            return new LevelAsset(
                name, 
                data.GetProperty<string>("LevelData"),
                data.GetProperty<LevelDataFormat>("LevelDataFormat"),
                data.GetProperty<string>("SourcePath"));
        }
    }
}