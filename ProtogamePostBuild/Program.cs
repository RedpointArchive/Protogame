using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Mono.Cecil;
using ProtoBuf.Meta;
using ProtoBuf.Precompile;

namespace ProtogamePostBuild
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var intermediateAssembly = args[0];
            var referencedAssemblies = args[1].Split(';');

            ImplementNetworkMessageSerializer(intermediateAssembly, referencedAssemblies);
        }

        private static void ImplementNetworkMessageSerializer(string intermediateAssembly, string[] referencedAssemblies)
        {
            // First generate a temporary assembly that contains the compiled protobuf serializer.  Using
            // a precompiled serializer dramatically improves performance.
            var temporaryAssemblyName = intermediateAssembly + ".NetworkIntermediate.dll";
            var context = new PreCompileContext();
            context.AssemblyName = temporaryAssemblyName;
            context.ProbePaths.AddRange(referencedAssemblies.Select(Path.GetDirectoryName));
            context.Inputs.Add(intermediateAssembly + ".NetworkSource.dll");
            context.Accessibility = RuntimeTypeModel.Accessibility.Public;
            context.TypeName = "IntermediateNetworkSerializer";
            context.Execute();

            // If the file doesn't exist, there were no network types to generate.
            if (!File.Exists(temporaryAssemblyName))
            {
                return;
            }

            // Otherwise, open the intermediate assembly and the temporary assembly and merge the serializer
            // into the intermediate assembly so we don't need a seperate DLL.
            try
            {
                var resolver = new PreloadedAssemblyResolver();
                foreach (var reference in referencedAssemblies)
                {
                    Console.WriteLine("Loading referenced assembly from " + reference + "...");
                    resolver.Load(reference);
                }
                
                var readSymbols =
                    File.Exists(Path.Combine(Path.GetDirectoryName(intermediateAssembly) ?? string.Empty,
                        Path.GetFileNameWithoutExtension(intermediateAssembly) + ".pdb")) ||
                    File.Exists(Path.Combine(Path.GetDirectoryName(intermediateAssembly) ?? string.Empty,
                        Path.GetFileNameWithoutExtension(intermediateAssembly) + ".dll.mdb"));
                var temporaryAssembly = AssemblyDefinition.ReadAssembly(temporaryAssemblyName,
                    new ReaderParameters { ReadSymbols = false, AssemblyResolver = resolver });
                var targetAssembly = AssemblyDefinition.ReadAssembly(intermediateAssembly,
                    new ReaderParameters { ReadSymbols = readSymbols, AssemblyResolver = resolver });

                var serializer = temporaryAssembly.MainModule.Types.First(x => x.Name == "IntermediateNetworkSerializer");
                var serializerCopy = new TypeDefinition("NetworkSerializers", (Path.GetFileNameWithoutExtension(intermediateAssembly) ?? string.Empty).Replace(".", "") + "Serializer", serializer.Attributes);
                targetAssembly.MainModule.Types.Add(serializerCopy);
                Console.WriteLine("Created type " + serializerCopy.FullName + ".");

                foreach (var method in serializer.Methods)
                {
                    var methodCopy = new MethodDefinition(method.Name, method.Attributes, targetAssembly.MainModule.Import(method.ReturnType));
                    serializerCopy.Methods.Add(methodCopy);
                    Console.WriteLine("Copied method " + methodCopy.FullName + ".");
                }

                try
                {
                    targetAssembly.Write(intermediateAssembly, new WriterParameters {WriteSymbols = false});
                    Console.WriteLine("Saved modified assembly.");
                }
                catch (IOException)
                {
                    Console.WriteLine("Unable to write new assembly file; is it in use by your IDE?");
                }
            }
            finally
            {
                try
                {
                    File.Delete(temporaryAssemblyName);
                }
                catch { }
            }
        }
    }
}