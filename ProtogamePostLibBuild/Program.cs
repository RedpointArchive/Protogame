namespace ProtogamePostBuild
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var intermediateAssembly = args[0];
            var referencedAssemblies = args[1].Split(';');

            var networkMessageSerializer = new NetworkMessageSerializer();
            networkMessageSerializer.ImplementNetworkMessageSerializer(intermediateAssembly, referencedAssemblies);
        }
    }
}