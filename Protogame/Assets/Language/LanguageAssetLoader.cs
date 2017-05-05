#if FALSE

namespace Protogame
{
    public class LanguageAssetLoader : IAssetLoader
    {
        public bool CanLoad(IRawAsset data)
        {
            return data.GetProperty<string>("Loader") == typeof(LanguageAssetLoader).FullName;
        }

        public IAsset Load(string name, IRawAsset data)
        {
            // The text key is the asset name.
            return new LanguageAsset(name, data.GetProperty<string>("Value"));
        }
    }
}

#endif