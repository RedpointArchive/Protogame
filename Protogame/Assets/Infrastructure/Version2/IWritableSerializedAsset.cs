namespace Protogame
{
    public interface IWritableSerializedAsset
    {
        void SetByteArray(string property, byte[] value);
    }
}
