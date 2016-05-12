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

            var networkMessageSerializer = new NetworkMessageSerializer();
            networkMessageSerializer.ImplementNetworkMessageSerializer(intermediateAssembly, referencedAssemblies);

            var atfLevelEditorBaking = new ATFLevelEditorBaking();
            atfLevelEditorBaking.BakeOutSchemaFile(intermediateAssembly, referencedAssemblies);
        }
    }
}