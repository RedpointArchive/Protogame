#if FALSE

using Protoinject;

namespace Protogame
{
    public class TilesetAssetLoader : IAssetLoader
    {
        private readonly IKernel _kernel;

        public TilesetAssetLoader(IKernel kernel)
        {
            _kernel = kernel;
        }

        public bool CanLoad(IRawAsset data)
        {
            return data.GetProperty<string>("Loader") == typeof(TilesetAssetLoader).FullName;
        }

        public IAsset Load(string name, IRawAsset data)
        {
            return new TilesetAsset(
                _kernel.Get<IAssetManager>(),
                name,
                data.GetProperty<string>("TextureName"),
                data.GetProperty<int>("CellWidth"),
                data.GetProperty<int>("CellHeight"));
        }
    }
}

#endif