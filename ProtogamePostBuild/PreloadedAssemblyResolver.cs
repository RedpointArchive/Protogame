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
            var assembly = AssemblyDefinition.ReadAssembly(path);
            _loadedAssemblies[assembly.Name] = assembly;
            AddSearchDirectory(new FileInfo(path).DirectoryName);
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

        public override AssemblyDefinition Resolve(string fullName)
        {
            if (!_loadedAssemblies.ContainsKey(AssemblyNameReference.Parse(fullName)))
            {
                return base.Resolve(fullName);
            }

            return _loadedAssemblies[AssemblyNameReference.Parse(fullName)];
        }

        public override AssemblyDefinition Resolve(string fullName, ReaderParameters parameters)
        {
            if (!_loadedAssemblies.ContainsKey(AssemblyNameReference.Parse(fullName)))
            {
                return base.Resolve(fullName, parameters);
            }

            return _loadedAssemblies[AssemblyNameReference.Parse(fullName)];
        }
    }
}