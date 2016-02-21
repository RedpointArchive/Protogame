using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
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
            File.Copy(intermediateAssembly, intermediateAssembly + ".NetworkSource.dll", true);
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

                targetAssembly.Modules.Add(temporaryAssembly.MainModule);

                var targetModule = targetAssembly.Modules.Last();
                targetModule.Name = "NetworkSerializersModule";

                // Get the serializer type.
                var baseSerializer = targetModule.Types.First(x => x.Name == "IntermediateNetworkSerializer");

                // Strip the sealed attribute off it.
                baseSerializer.Attributes &= ~TypeAttributes.Sealed;

                // Now create a new class in the module which inherits from the target class.
                var netSerializer = new TypeDefinition("_NetworkSerializers", "<>GeneratedSerializer",
                    TypeAttributes.AnsiClass | TypeAttributes.AutoClass | TypeAttributes.BeforeFieldInit | TypeAttributes.Public);
                netSerializer.BaseType = targetAssembly.MainModule.Import(baseSerializer);
                targetAssembly.MainModule.Types.Add(netSerializer);

                // Create a constructor in that class.
                var netConstructor = new MethodDefinition(".ctor", MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName, targetAssembly.MainModule.Import(typeof(void)));
                netConstructor.Body.MaxStackSize = 8;
                netConstructor.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
                netConstructor.Body.Instructions.Add(Instruction.Create(OpCodes.Call, targetAssembly.MainModule.Import(baseSerializer.Methods.First(x => x.Name == ".ctor"))));
                netConstructor.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));
                netSerializer.Methods.Add(netConstructor);

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