using System;

namespace Protogame
{
    public class ModelAsset : IAsset
    {
        private readonly IModelSerializer _modelSerializer;
        private readonly byte[] _data;
        
        public ModelAsset(IModelSerializer modelSerializer, string name, byte[] data)
        {
            _modelSerializer = modelSerializer;
            Name = name;
            _data = data;
        }

        public string Name { get; private set; }

        /// <summary>
        /// Instantiates a model object from the model asset.  Each model object has it's own vertex and index
        /// buffers, so you should aim to share model objects which will always have the same state.
        /// </summary>
        /// <returns>The model object.</returns>
        public IModel InstantiateModel()
        {
            if (_data != null)
            {
                return _modelSerializer.Deserialize(Name, _data);
            }

            throw new InvalidOperationException("Attempted to instantiate a model, but no platform data was loaded.");
        }
    }
}