namespace ProtogameDocsTool
{
    public class DocumentationToProcess
    {
        public DocumentationToProcess(string assembly, string doc, bool supportsModules)
        {
            AssemblyFile = assembly;
            DocumentationFile = doc;
            SupportsModules = supportsModules;
        }

        public string AssemblyFile { get; set; }

        public string DocumentationFile { get; set; }

        public bool SupportsModules { get; set; }
    }
}

