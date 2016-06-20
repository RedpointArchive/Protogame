namespace ProtogamePostBuild
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var intermediateAssembly = args[0];
            var referencedAssemblies = args[1].Split(';');

            var atfLevelEditorBaking = new ATFLevelEditorBaking();
            atfLevelEditorBaking.BakeOutSchemaFile(intermediateAssembly, referencedAssemblies);
        }
    }
}