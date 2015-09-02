using System.Collections.Generic;

namespace ProtogameDocsTool
{
    public class DocumentationList : List<DocumentationToProcess>
    {
        public void Add(string assembly, string doc, bool supportsModule)
        {
            base.Add(new DocumentationToProcess(assembly, doc, supportsModule));
        }
    }
}

