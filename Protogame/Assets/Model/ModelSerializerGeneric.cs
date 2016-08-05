using System;
using System.IO;

namespace Protogame
{
    /// <summary>
    /// Serializes and deserializes <see cref="IModel"/> for storage in a binary format.
    /// </summary>
    public class ModelSerializerGeneric : IModelSerializer
    {
        private readonly ModelSerializerVersion1 _modelSerializerVersion1;
        private readonly ModelSerializerVersion2 _modelSerializerVersion2;
        private readonly ModelSerializerVersion3 _modelSerializerVersion3;

        public ModelSerializerGeneric(
            ModelSerializerVersion1 modelSerializerVersion1,
            ModelSerializerVersion2 modelSerializerVersion2,
            ModelSerializerVersion3 modelSerializerVersion3)
        {
            _modelSerializerVersion1 = modelSerializerVersion1;
            _modelSerializerVersion2 = modelSerializerVersion2;
            _modelSerializerVersion3 = modelSerializerVersion3;
        }

        /// <summary>
        /// Deserialize the specified byte array into a concrete <see cref="Model"/> implementation.
        /// </summary>
        /// <param name="data">
        /// The byte array to deserialize.
        /// </param>
        /// <returns>
        /// The deserialized <see cref="Model"/>.
        /// </returns>
        public Model Deserialize(string name, byte[] data)
        {
            using (var memory = new MemoryStream(data))
            {
                using (var reader = new BinaryReader(memory))
                {
                    // The first version of the model format didn't have a version number; instead
                    // the first read value in that format is the number of animations.  So we use a 
                    // very large number that would never be a real number of animations to indicate
                    // that the format is now versioned.
                    if (reader.ReadInt32() == Int32.MaxValue)
                    {
                        var version = reader.ReadInt32();
                        switch (version)
                        {
                            case 2:
                                return _modelSerializerVersion2.Deserialize(name, data);
                            case 3:
                                return _modelSerializerVersion3.Deserialize(name, data);
                            default:
                                throw new InvalidOperationException("Unknown version for model format.");
                        }
                    }

                    // No version signature, so it must be version 1.
                    return _modelSerializerVersion1.Deserialize(name, data);
                }
            }
        }

        /// <summary>
        /// Serializes the specified <see cref="IModel"/> into a byte array.
        /// </summary>
        /// <param name="model">
        /// The model to serialize.
        /// </param>
        /// <returns>
        /// The resulting byte array.
        /// </returns>
        public byte[] Serialize(IModel model)
        {
            return _modelSerializerVersion3.Serialize(model);
        }
    }
}