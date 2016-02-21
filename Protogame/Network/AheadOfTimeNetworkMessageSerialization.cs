namespace Protogame
{
    public class AheadOfTimeNetworkMessageSerialization : INetworkMessageSerialization
    {
        private bool _initialized;

        public byte[] Serialize(object message)
        {
            if (!_initialized)
            {
                Init();
            }
        }

        public object Deserialize(byte[] data)
        {
            if (!_initialized)
            {
                Init();
            }
        }

        private void Init()
        {
            // search through all assemblies for network serializers...
        }
    }
}