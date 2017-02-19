namespace Protogame
{
    public class ModelAssetLoader : IAssetLoader
    {
        private readonly IModelSerializer _modelSerializer;

        public ModelAssetLoader(IModelSerializer modelSerializer)
        {
            _modelSerializer = modelSerializer;
        }
        
        public bool CanLoad(IRawAsset data)
        {
            return data.GetProperty<string>("Loader") == typeof(ModelAssetLoader).FullName;
        }

        public IAsset Load(string name, IRawAsset data)
        {
            if (data is CompiledAsset)
            {
                return new ModelAsset(_modelSerializer, name, null, null, data.GetProperty<PlatformData>("PlatformData"), false, string.Empty, null);
            }

            PlatformData platformData = null;
            if (data.GetProperty<PlatformData>("PlatformData") != null)
            {
                platformData = new PlatformData
                {
                    Platform = data.GetProperty<PlatformData>("PlatformData").Platform,
                    Data = data.GetProperty<PlatformData>("PlatformData").Data
                };
            }

            var model = new ModelAsset(
                _modelSerializer,
                name, 
                ByteReader.ReadAsByteArray(data.GetProperty<object>("RawData")),
                data.GetProperty<System.Collections.Generic.Dictionary<string, byte[]>>("RawAdditionalAnimations"), 
                platformData,
                data.GetProperty<bool>("SourcedFromRaw"),
                data.GetProperty<string>("Extension"),
                data.GetProperty<string[]>("ImportOptions"));

            return model;
        }
    }
}