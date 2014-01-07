using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ProtogameAssetTool
{
    public static class Stub32
    {
        public static string Call(Type type, string method, string arg)
        {
            var progDir = new FileInfo(Assembly.GetExecutingAssembly().Location).Directory.FullName;
            var prog = Path.Combine(progDir, "ProtogameAssetTool32.exe");

            var process = new Process();
            process.StartInfo = new ProcessStartInfo
            {
                Arguments =
                    "\"" + type.Assembly.Location + "\" \"" + type.FullName + "\" \"" + method + "\" \"" + arg + "\"",
                FileName = prog,
                RedirectStandardOutput = true,
                UseShellExecute = false
            };
            process.Start();
            var output = process.StandardOutput.ReadToEnd().Trim();
            process.WaitForExit();
            return output;
        }
    }
}
