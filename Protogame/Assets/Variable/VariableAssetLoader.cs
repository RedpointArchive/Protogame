namespace Protogame
{
    public class VariableAssetLoader : IAssetLoader
    {
        public bool CanLoad(IRawAsset data)
        {
            return data.GetProperty<string>("Loader") == typeof(VariableAssetLoader).FullName;
        }

        public IAsset Load(string name, IRawAsset data)
        {
            return new VariableAsset(name, data.GetProperty<object>("Value"));
        }
    }
}