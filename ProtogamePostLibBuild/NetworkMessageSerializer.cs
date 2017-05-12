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
    public class NetworkMessageSerializer
    {
        public void ImplementNetworkMessageSerializer(string intermediateAssembly, string[] referencedAssemblies)
        {
            // First generate a temporary assembly that contains the compiled protobuf serializer.  Using
            // a precompiled serializer dramatically improves performance.
            File.Copy(intermediateAssembly, intermediateAssembly + ".NetworkSource.dll", true);
            var temporaryAssemblyName = intermediateAssembly + ".NetworkIntermediate.dll";
            var context = new PreCompileContext();
            context.AssemblyName = temporaryAssemblyName;
            context.ProbePaths.AddRange(referencedAssemblies.Select(Path.GetDirectoryName));
            context.Inputs.Add(intermediateAssembly + ".NetworkSource.dll");
            context.TypeName = "_NetworkSerializer";
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
                    new ReaderParameters { ReadSymbols = false, AssemblyResolver = resolver, InMemory = true });
                var targetAssembly = AssemblyDefinition.ReadAssembly(intermediateAssembly,
                    new ReaderParameters { ReadSymbols = readSymbols, AssemblyResolver = resolver, InMemory = true });

                // This copies just enough for the protobuf serializer to copy across (e.g.
                // it doesn't support generics or anything fancy).
                var sourceType = temporaryAssembly.MainModule.GetType("_NetworkSerializer");
                var targetType = new TypeDefinition("_NetworkSerializers", "<>GeneratedSerializer", sourceType.Attributes, targetAssembly.MainModule.Import(sourceType.BaseType));
                targetAssembly.MainModule.Types.Add(targetType);
                CopyType(sourceType, targetType);
                
                try
                {
                    targetAssembly.Write(intermediateAssembly, new WriterParameters {WriteSymbols = readSymbols});
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

        private static void CopyType(TypeDefinition sourceType, TypeDefinition targetType)
        {
            foreach (var field in sourceType.Fields)
            {
                CopyFieldTo(field, targetType);
            }

            foreach (var method in sourceType.Methods)
            {
                CopyMethodTo(method, targetType);
            }
        }

        private static void CopyFieldTo(FieldDefinition field, TypeDefinition targetType)
        {
            var target = new FieldDefinition(field.Name, field.Attributes, targetType.Module.Import(field.FieldType));
            targetType.Fields.Add(target);

            Console.WriteLine("Copied field " + target.Name + ".");
        }

        private static void CopyMethodTo(MethodDefinition method, TypeDefinition targetType)
        {
            var target = new MethodDefinition(method.Name, method.Attributes, targetType.Module.Import(method.ReturnType));
            targetType.Methods.Add(target);

            foreach (var @override in method.Overrides)
            {
                var targetOverride = targetType.Module.Import(@override);
                target.Overrides.Add(targetOverride);
            }

            foreach (var parameter in method.Parameters)
            {
                var targetParameter = new ParameterDefinition(parameter.Name, parameter.Attributes, targetType.Module.Import(parameter.ParameterType));
                target.Parameters.Add(targetParameter);
            }

            foreach (var variable in method.Body.Variables)
            {
                var targetVariable = new VariableDefinition(variable.Name, targetType.Module.Import(variable.VariableType));
                target.Body.Variables.Add(targetVariable);
            }

            foreach (var instruction in method.Body.Instructions)
            {
                if (instruction.Operand is FieldReference)
                {
                    var f = (FieldReference) instruction.Operand;
                    if (f.DeclaringType == method.DeclaringType)
                    {
                        instruction.Operand = targetType.Fields.FirstOrDefault(x => x.Name == f.Name);
                    }
                    else
                    {
                        instruction.Operand =
                            targetType.Module.GetType(f.DeclaringType.FullName)
                                .Fields.FirstOrDefault(x => x.Name == f.Name);
                    }
                    if (instruction.Operand == null)
                    {
                        Console.WriteLine("WARNING: Unable to resolve field " + f.Name + " in new type.  The network serializer will probably crash :(");
                    }
                }
                else if (instruction.Operand is ParameterReference)
                {
                    var p = (ParameterReference) instruction.Operand;
                    instruction.Operand = target.Parameters[p.Index];
                }
                else if (instruction.Operand is MethodReference)
                {
                    var m = (MethodReference) instruction.Operand;
                    if (m.DeclaringType.Name == "_NetworkSerializer")
                    {
                        // redirect to this type.
                        var om = m;
                        m = new MethodReference(m.Name, targetType.Module.Import(m.ReturnType), targetType);
                        foreach (var oldArg in om.Parameters)
                        {
                            m.Parameters.Add(new ParameterDefinition(oldArg.Name, oldArg.Attributes, targetType.Module.Import(oldArg.ParameterType)));
                        }
                    }
                    else
                    {
                        m = targetType.Module.Import(m);
                    }
                    instruction.Operand = m;
                }
                else if (instruction.Operand is TypeReference)
                {
                    instruction.Operand = targetType.Module.Import((TypeReference)instruction.Operand);
                }

                target.Body.Instructions.Add(instruction);

                //var copiedInstruction = instruction;
            }

            Console.WriteLine("Copied method " + target.Name + ".");
        }
    }
}