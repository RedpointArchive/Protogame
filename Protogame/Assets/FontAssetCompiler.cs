using System.Text;
using Newtonsoft.Json;

#if PLATFORM_WINDOWS || PLATFORM_LINUX
namespace Protogame
{
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework.Content.Pipeline;
    using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
    using MonoGame.Framework.Content.Pipeline.Builder;

#if PLATFORM_WINDOWS
    using System.CodeDom.Compiler;
    using Microsoft.CSharp;
#endif

    public class FontAssetCompiler : MonoGameContentCompiler, IAssetCompiler<FontAsset>
    {
        public void Compile(FontAsset asset, TargetPlatform platform)
        {
#if PLATFORM_WINDOWS
            if (IntPtr.Size != 4)
            {
                // Compilation of SpriteFonts requires that the process
                // is executing under 32-bit due to native dependencies.
                this.CompileFontUnder32BitProcess(asset, platform);
            }
            else
            {
                this.CompileFont(asset, platform);
            }
#else
            this.CompileFont(asset, platform);
#endif
        }

        private FontDescription GetDescriptionForAsset(FontAsset asset)
        {
            var chars = new List<CharacterRegion>();
            chars.Add(new CharacterRegion(' ', '~'));
            var fontName = string.IsNullOrEmpty(asset.FontName) ? "Arial" : asset.FontName;
            var fontDesc = new FontDescription(
                fontName,
                asset.FontSize,
                asset.Spacing,
                FontDescriptionStyle.Regular,
                asset.UseKerning,
                chars);
#if PLATFORM_LINUX
            fontDesc.Identity = new ContentIdentity
            {
                SourceFilename = "/usr/share/fonts/truetype/dummy.spritefont"
            };
#endif
            return fontDesc;
        }

        private void CompileFont(FontAsset asset, TargetPlatform platform)
        {
            try
            {
                var manager = new PipelineManager(Environment.CurrentDirectory, Environment.CurrentDirectory, Environment.CurrentDirectory);
                var dictionary = new OpaqueDataDictionary();
                var processor = manager.CreateProcessor("FontDescriptionProcessor", dictionary);
                var context = new DummyContentProcessorContext(TargetPlatformCast.ToMonoGamePlatform(platform));
                var content = processor.Process(this.GetDescriptionForAsset(asset), context);

                asset.PlatformData = new PlatformData
                {
                    Platform = platform,
                    Data = this.CompileAndGetBytes(content)
                };

                try
                {
                    asset.ReloadFont();
                }
                catch (NoAssetContentManagerException)
                {
                    // We might be running under a server where we can't load
                    // the actual texture (because we have no game).
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                // The user might not have the font installed...
            }
            catch (NullReferenceException)
            {
                // The user might not have the font installed...
            }
        }

#if PLATFORM_WINDOWS

        private static string m_CompiledTool = null;

        private static string BuildCompilationTool()
        {
            if (m_CompiledTool != null)
            {
                return m_CompiledTool;
            }

            var random = new Random();
            var directory = Path.Combine(Path.GetTempPath(), "fontcompile_" + random.Next());
            while (Directory.Exists(directory))
            {
                directory = Path.Combine(Path.GetTempPath(), "fontcompile_" + random.Next());
            }
            Directory.CreateDirectory(directory);

            // Copy dependencies first.
            var currentDirectory = new FileInfo(typeof(FontAssetCompiler).Assembly.Location).Directory.FullName;
            var dependencies = new[]
            {
                "Protogame.dll",
                "freetype6.dll",
                "Newtonsoft.Json.dll",
                "MonoGame.Framework.Content.Pipeline.dll",
                "MonoGame.Framework.dll",
                "protobuf-net.dll",
                "SharpDX.D3DCompiler.dll",
                "SharpDX.dll",
                "Nvidia.TextureTools.dll",
                "nvtt.dll",
                "SharpFont.dll",
            };

            foreach (var dependency in dependencies)
            {
                File.Copy(Path.Combine(currentDirectory, dependency), Path.Combine(directory, dependency));
            }

            var codeProvider = new CSharpCodeProvider();
            var parameters = new CompilerParameters(new[] { "System.dll", typeof(FontAssetCompiler).Assembly.Location });
            parameters.GenerateExecutable = true;
            parameters.CompilerOptions = "/platform:x86";
            parameters.OutputAssembly = Path.Combine(directory, "fontcompile32.exe");
            var results = codeProvider.CompileAssemblyFromSource(parameters, @"
using System;
using Protogame;

public static class Program
{
    public static void Main(string[] args)
    {
        var compiler = new FontAssetCompiler();
        Console.WriteLine(compiler.CompileFontFrom32BitArgument(args[0]));
    }
}
");
            if (results.Errors.Count > 0)
            {
                throw new InvalidOperationException("Unable to build font compilation tool.");
            }

            m_CompiledTool = parameters.OutputAssembly;

            AppDomain.CurrentDomain.ProcessExit += (sender, args) =>
            {
                foreach (var dependency in dependencies)
                {
                    try
                    {
                        File.Delete(Path.Combine(directory, dependency));
                    }
                    // ReSharper disable once EmptyGeneralCatchClause
                    catch
                    {
                    }
                }

                try
                {
                    File.Delete(m_CompiledTool);
                    Directory.Delete(directory);
                }
                // ReSharper disable once EmptyGeneralCatchClause
                catch
                {
                }
            };

            return m_CompiledTool;
        }

        /// <remarks>
        /// When running under Windows and a 64-bit process, we have to execute the compilation
        /// under a 32-bit process.
        /// <para>
        /// Previously we did this by shipping a "ProtogameAssetTool32"
        /// stub that let use execute code explicitly under a 32-bit process, but this only
        /// worked for the bulk asset compiler and wasn't practical for the asset manager
        /// or games.  Now we compile code using the C# compiler, save it as a 32-bit
        /// assembly, and use that assembly to perform the compilation.  We don't need
        /// to worry about the C# compiler dependency, as this step is *only* required on
        /// Windows; Linux and other platforms have a 64-bit FreeType library available so
        /// it isn't an issue there.
        /// </para>
        /// </remarks>
        private void CompileFontUnder32BitProcess(FontAsset asset, TargetPlatform platform)
        {
            var buildTool = BuildCompilationTool();

            // Create the arguments.  We make a copy of the asset in case the asset has platform data
            // attached (which we don't need and can't send over the command line because of the size).
            var arguments = new FontCompilationArguments
            {
                FontAsset = new FontAsset(
                    null,
                    asset.Name,
                    asset.FontName,
                    asset.FontSize,
                    asset.UseKerning,
                    asset.Spacing,
                    null),
                TargetPlatform = platform
            };
            var encodedArguments = Convert.ToBase64String(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(arguments)));

            var process = new Process();
            process.StartInfo = new ProcessStartInfo
            {
                Arguments = encodedArguments,
                FileName = buildTool,
                RedirectStandardOutput = true,
                UseShellExecute = false
            };
            process.Start();
            var output = process.StandardOutput.ReadToEnd().Trim();
            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                throw new InvalidOperationException("Font compilation process crashed.");
            }

            if (output.Trim() == string.Empty)
            {
                // User might not have the font installed.
                return;
            }

            var data = Convert.FromBase64String(output);

            asset.PlatformData = new PlatformData
            {
                Platform = platform,
                Data = data
            };

            try
            {
                asset.ReloadFont();
            }
            catch (NoAssetContentManagerException)
            {
                // We might be running under a server where we can't load
                // the actual texture (because we have no game).
            }
        }

        public string CompileFontFrom32BitArgument(string arg)
        {
            var arguments =
                JsonConvert.DeserializeObject<FontCompilationArguments>(
                    Encoding.ASCII.GetString(Convert.FromBase64String(arg)));

            try
            {
                var manager = new PipelineManager(Environment.CurrentDirectory, Environment.CurrentDirectory, Environment.CurrentDirectory);
                var dictionary = new OpaqueDataDictionary();
                var processor = manager.CreateProcessor("FontDescriptionProcessor", dictionary);
                var context = new DummyContentProcessorContext(TargetPlatformCast.ToMonoGamePlatform(arguments.TargetPlatform));
                var content = processor.Process(this.GetDescriptionForAsset(arguments.FontAsset), context);

                return Convert.ToBase64String(this.CompileAndGetBytes(content));
            }
            catch (NullReferenceException)
            {
                // The user might not have the font installed...
            }

            return string.Empty;
        }

        private class FontCompilationArguments
        {
            public FontAsset FontAsset { get; set; }

            public TargetPlatform TargetPlatform { get; set; }
        }

#endif

    }
}

#endif