using System.Collections.Generic;
using System.IO;
using Mono.Cecil;

namespace ProtogamePostBuild
{
    public class PreloadedAssemblyResolver : DefaultAssemblyResolver
    {
        private readonly Dictionary<AssemblyNameReference, AssemblyDefinition> _loadedAssemblies;

        public PreloadedAssemblyResolver()
        {
            _loadedAssemblies = new Dictionary<AssemblyNameReference, AssemblyDefinition>();
        }

        public void Load(string path)
        {
            if (File.Exists(path))
            {
                var assembly = AssemblyDefinition.ReadAssembly(path);
                _loadedAssemblies[assembly.Name] = assembly;
                AddSearchDirectory(new FileInfo(path).DirectoryName);
            }
        }

        public override AssemblyDefinition Resolve(AssemblyNameReference name)
        {
            if (!_loadedAssemblies.ContainsKey(name))
            {
                return base.Resolve(name);
            }

            return _loadedAssemblies[name];
        }

        public override AssemblyDefinition Resolve(AssemblyNameReference name, ReaderParameters parameters)
        {
            if (!_loadedAssemblies.ContainsKey(name))
            {
                return base.Resolve(name, parameters);
            }

            return _loadedAssemblies[name];
        }
    }
}