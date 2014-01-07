using System;
using System.Diagnostics;
using System.Reflection;

namespace ProtogameAssetTool32
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine(
@"This is a stub program that is required by ProtogameAssetTool to 
compile some types of assets (namely fonts) due to 32-bit dependencies.");
            }

            var assemblyName = args[0];
            var typeName = args[1];
            var methodName = args[2];
            var argument = args[3];

            var assembly = Assembly.LoadFile(assemblyName);
            var type = assembly.GetType(typeName, true);
            var method = type.GetMethod(methodName, BindingFlags.Public | BindingFlags.Static);
            Console.WriteLine((string)method.Invoke(null, new object[] { argument }));
        }
    }
}
