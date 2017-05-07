using Protogame;
using Protoinject;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace ProtogameAssetTool
{
    public class ServerOperation : IOperation
    {
        private SemaphoreSlim _outboundSemaphore;
        private TcpClient _inboundClient;
        private TcpClient _outboundClient;
        private BinaryReader _inboundReader;
        private BinaryWriter _inboundWriter;
        private BinaryReader _outboundReader;
        private BinaryWriter _outboundWriter;
        private ICompiledAssetFs _compiledFs;

        public async Task Run(OperationArguments args)
        {
            _outboundSemaphore = new SemaphoreSlim(1);

            await ForwardPorts(args).ConfigureAwait(false);

            _compiledFs = await SetupCompiledFs(args).ConfigureAwait(false);
            _compiledFs.RegisterUpdateNotifier(OnAssetUpdated);

            await ServiceDevice().ConfigureAwait(false);
        }

        private async Task OnAssetUpdated(string assetName)
        {
            if (_outboundClient?.Connected ?? false)
            {
                // Notify via the outbound channel that one of the assets has been updated.
                await _outboundSemaphore.WaitAsync().ConfigureAwait(false);
                try
                {
                    var asset = await _compiledFs.Get(assetName).ConfigureAwait(false);
                    if (asset == null)
                    {
                        Console.WriteLine("Asset " + assetName + " has been deleted, notifying...");
                        _outboundWriter.Write("deleted-asset");
                        _outboundWriter.Write(assetName);
                    }
                    else
                    {
                        Console.WriteLine("Asset " + assetName + " has been updated, notifying...");
                        _outboundWriter.Write("updated-asset");
                        await WriteAssetFsFileToStream(_outboundWriter, asset).ConfigureAwait(false);
                    }
                    _outboundWriter.Flush();

                    Console.WriteLine("Notification complete");
                }
                finally
                {
                    _outboundSemaphore.Release();
                }
            }
        }

        private async Task ForwardPorts(OperationArguments args)
        {
            Console.WriteLine("Forwarding port...");
            Console.WriteLine(args.AndroidPlatformTools);
            var adb = Path.Combine(args.AndroidPlatformTools, "adb.exe");
            var p = Process.Start(adb, "forward tcp:23400 tcp:23400");
            p.WaitForExit();
            p = Process.Start(adb, "forward tcp:23401 tcp:23401");
            p.WaitForExit();
        }

        private async Task<ICompiledAssetFs> SetupCompiledFs(OperationArguments args)
        {
            var platform = args.Platforms.First();
            var targetPlatform = (TargetPlatform)Enum.Parse(typeof(TargetPlatform), platform);

            var sourcePath = Path.Combine(Environment.CurrentDirectory, "assets", ".source");
            if (!File.Exists(sourcePath))
            {
                throw new InvalidOperationException("No source asset folder!  Make sure at least one folder in your content project has Source=\"True\"!");
            }
            using (var reader = new StreamReader(sourcePath))
            {
                sourcePath = reader.ReadLine();
            }

            Console.WriteLine("Hosting compiled assets for " + platform + " platform...");

            var kernel = new StandardKernel();
            kernel.Load<ProtogameAssetModule>();
            kernel.Unbind<IAssetFsLayer>();
            kernel.Bind<IAssetFsLayer>().ToMethod(ctx => ctx.Kernel.Get<LocalFilesystemAssetFsLayer>(new NamedConstructorArgument("basePath", sourcePath)));
            kernel.Bind<IAssetFsLayer>().ToMethod(ctx => ctx.Kernel.Get<LocalFilesystemAssetFsLayer>(new NamedConstructorArgument("basePath", Path.Combine(Environment.CurrentDirectory, "assets", targetPlatform.ToString()))));
            kernel.Unbind<ICompiledAssetFs>();
            kernel.Rebind<ICompiledAssetFs>().ToMethod(ctx => ctx.Kernel.Get<HostCompiledAssetFs>(new NamedConstructorArgument("targetPlatform", targetPlatform)));
            kernel.Rebind<IRenderBatcher>().To<NullRenderBatcher>();
            kernel.Unbind<IAssetCompiler>();
            (new ProtogameAssetModule()).LoadRawAssetStrategies(kernel);
            CompileOperation.LoadAndBindTypes(kernel, args.Assemblies);

            return kernel.Get<ICompiledAssetFs>();
        }

        private async Task ServiceDevice()
        {
            await ConnectToRemoteDevice().ConfigureAwait(false);

            while (true)
            {
                if (!_inboundClient.Connected || !_outboundClient.Connected)
                {
                    await ConnectToRemoteDevice().ConfigureAwait(false);
                    continue;
                }

                _inboundReader = new BinaryReader(_inboundClient.GetStream());
                _inboundWriter = new BinaryWriter(_inboundClient.GetStream());
                _outboundReader = new BinaryReader(_outboundClient.GetStream());
                _outboundWriter = new BinaryWriter(_outboundClient.GetStream());

                try
                {
                    Console.WriteLine("Listening for messages...");
                    while (true)
                    {
                        var request = _inboundReader.ReadString();
                        switch (request)
                        {
                            case "list-assets":
                                {
                                    Console.WriteLine("Request: list-assets");
                                    await _outboundSemaphore.WaitAsync().ConfigureAwait(false);
                                    try
                                    {
                                        var assets = await _compiledFs.List().ConfigureAwait(false);
                                        _inboundWriter.Write((Int32)assets.Length);
                                        for (var i = 0; i < assets.Length; i++)
                                        {
                                            await WriteAssetFsFileToStream(_inboundWriter, assets[i]).ConfigureAwait(false);
                                        }
                                        _inboundWriter.Flush();
                                    }
                                    finally
                                    {
                                        _outboundSemaphore.Release();
                                    }
                                    Console.WriteLine("Request complete: list-assets");
                                    break;
                                }
                            case "get-asset-content":
                                {
                                    var assetName = _inboundReader.ReadString();
                                    Console.WriteLine("Request: get-asset-content " + assetName);
                                    await _outboundSemaphore.WaitAsync();
                                    try
                                    {
                                        var asset = await _compiledFs.Get(assetName).ConfigureAwait(false);
                                        if (asset == null)
                                        {
                                            Console.WriteLine("Sent no data (no asset is available!)");
                                            _inboundWriter.Write(false);
                                        }
                                        else
                                        {
                                            using (var stream = await asset.GetContentStream().ConfigureAwait(false))
                                            {
                                                var b = new byte[stream.Length];
                                                await stream.ReadAsync(b, 0, b.Length).ConfigureAwait(false);
                                                _inboundWriter.Write(true);
                                                _inboundWriter.Write((Int32)b.Length);
                                                Console.WriteLine("Sending " + b.Length + " bytes");
                                                _inboundWriter.Write(b);
                                            }
                                        }
                                        _inboundWriter.Flush();
                                    }
                                    finally
                                    {
                                        _outboundSemaphore.Release();
                                    }
                                    Console.WriteLine("Request complete: get-asset-content " + assetName);
                                    break;
                                }
                            case "logs":
                                {
                                    var logCount = _inboundReader.ReadInt32();
                                    for (var i = 0; i < logCount; i++)
                                    {
                                        var logLevel = (ConsoleLogLevel)_inboundReader.ReadInt32();

                                        switch (logLevel)
                                        {
                                            case ConsoleLogLevel.Debug:
                                                Console.Write("[debug] ");
                                                break;
                                            case ConsoleLogLevel.Info:
                                                Console.Write("[info ] ");
                                                break;
                                            case ConsoleLogLevel.Warning:
                                                Console.Write("[warn ] ");
                                                break;
                                            case ConsoleLogLevel.Error:
                                                Console.Write("[error] ");
                                                break;
                                        }

                                        Console.WriteLine(_inboundReader.ReadString());
                                    }
                                    break;
                                }
                        }
                    }
                }
                catch (EndOfStreamException ex)
                {
                    await Task.Delay(5000).ConfigureAwait(false);

                    // Needs reconnection.
                    try
                    {
                        _inboundClient?.Close();
                        _outboundClient?.Close();
                    }
                    catch { }
                    await ConnectToRemoteDevice();
                    continue;
                }

                await Task.Delay(5000);
            }
        }

        private async Task WriteAssetFsFileToStream(BinaryWriter writer, IAssetFsFile assetFsFile)
        {
            writer.Write((string)assetFsFile.Name);
            writer.Write((string)assetFsFile.Extension);
            writer.Write((Int64)assetFsFile.ModificationTimeUtcTimestamp.Ticks);
            var dependents = await assetFsFile.GetDependentOnAssetFsFileNames().ConfigureAwait(false);
            writer.Write((Int32)dependents.Length);
            for (var i = 0; i < dependents.Length; i++)
            {
                writer.Write((string)dependents[i]);
            }
        }

        private async Task ConnectToRemoteDevice()
        {
            _inboundClient = null;
            _outboundClient = null;
            while (true)
            {
                try
                {
                    Console.WriteLine("Attempting to connect to remote device...");
                    _inboundClient = new TcpClient();
                    _outboundClient = new TcpClient();
                    await _inboundClient.ConnectAsync(IPAddress.Loopback, 23401).ConfigureAwait(false);
                    await _outboundClient.ConnectAsync(IPAddress.Loopback, 23400).ConfigureAwait(false);
                    break;
                }
                catch
                {
                    if (_inboundClient.Connected)
                    {
                        _inboundClient.Close();
                    }
                    if (_outboundClient.Connected)
                    {
                        _outboundClient.Close();
                    }

                    await Task.Delay(5000);
                }
            }
        }
    }
}
