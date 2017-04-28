namespace Protogame
{
    public interface ISerializedAsset
    {
        void SetByteArray(string property, byte[] value);

        byte[] GetByteArray(string property);
    }
}
