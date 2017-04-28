using System.Threading.Tasks;

namespace Protogame
{
    public class ModelAssetLoader : IAssetLoader<ModelAsset>
    {
        private readonly IModelSerializer _modelSerializer;

        public ModelAssetLoader(IModelSerializer modelSerializer)
        {
            _modelSerializer = modelSerializer;
        }
        
        public async Task<IAsset> Load(string name, SerializedAsset input, IAssetManager assetManager)
        {
            return new ModelAsset(
                _modelSerializer,
                name,
                input.GetByteArray("Data"));
        }
    }
}