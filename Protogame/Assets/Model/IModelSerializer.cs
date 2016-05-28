namespace Protogame
{
    /// <summary>
    /// Serializes and deserializes <see cref="IModel"/> for storage in a binary format.
    /// </summary>
    public interface IModelSerializer
    {
        /// <summary>
        /// Deserialize the specified byte array into a concrete <see cref="Model"/> implementation.
        /// </summary>
        /// <param name="data">
        /// The byte array to deserialize.
        /// </param>
        /// <returns>
        /// The deserialized <see cref="Model"/>.
        /// </returns>
        Model Deserialize(byte[] data);

        /// <summary>
        /// Serializes the specified <see cref="IModel"/> into a byte array.
        /// </summary>
        /// <param name="model">
        /// The model to serialize.
        /// </param>
        /// <returns>
        /// The resulting byte array.
        /// </returns>
        byte[] Serialize(IModel model);
    }
}