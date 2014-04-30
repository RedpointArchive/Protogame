namespace Protogame
{
    public class VariableAssetLoader : IAssetLoader
    {
        public bool CanHandle(IRawAsset data)
        {
            return data.GetProperty<string>("Loader") == typeof(VariableAssetLoader).FullName;
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
            return new VariableAsset(name, null);
        }

        public IAsset Handle(IAssetManager assetManager, string name, IRawAsset data)
        {
            return new VariableAsset(name, data.GetProperty<object>("Value"));
        }
    }
}