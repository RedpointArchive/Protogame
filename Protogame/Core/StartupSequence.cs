#if PLATFORM_WINDOWS || PLATFORM_MACOS || PLATFORM_LINUX
#define CAN_RUN_DEDICATED_SERVER
#endif

using Protoinject;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Protogame
{
    public static class StartupSequence
    {
        public static StartupSequenceResult Start(string[] args)
        {
            StartupTrace.WriteLine("Protected Startup: Execution of protected startup has begun");

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var kernel = new StandardKernel();
            kernel.Bind<IRawLaunchArguments>()
                .ToMethod(x => new DefaultRawLaunchArguments(args))
                .InSingletonScope();

            Func<System.Reflection.Assembly, Type[]> TryGetTypes = assembly =>
            {
                try
                {
                    return assembly.GetTypes();
                }
                catch
                {
                    return new Type[0];
                }
            };

            StartupTrace.WriteLine("Protected Startup: Scanning for implementations of IGameConfiguration");

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var typeSource = new List<Type>();
            foreach (var assembly in assemblies)
            {
                typeSource.AddRange(assembly.GetCustomAttributes(false).OfType<ConfigurationAttribute>().Select(x => x.GameConfigurationOrServerClass));
            }

            StartupTrace.TimingEntries["scanForConfigurationAttributes"] = stopwatch.Elapsed;
            stopwatch.Restart();

            if (typeSource.Count == 0)
            {
                // Scan all types to find implementors of IGameConfiguration
                typeSource.AddRange(from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                    from type in TryGetTypes(assembly)
                                    select type);

                StartupTrace.TimingEntries["scanForGameConfigurationImplementations"] = stopwatch.Elapsed;
                stopwatch.Restart();
            }

            // Search the application domain for implementations of
            // the IGameConfiguration.
            var gameConfigurations = new List<IGameConfiguration>();
            foreach (var type in typeSource)
            {
                if (typeof(IGameConfiguration).IsAssignableFrom(type) &&
                    !type.IsInterface && !type.IsAbstract)
                {
                    gameConfigurations.Add(Activator.CreateInstance(type) as IGameConfiguration);
                }
            }

            StartupTrace.WriteLine("Protected Startup: Found " + gameConfigurations.Count + " implementations of IGameConfiguration");

#if CAN_RUN_DEDICATED_SERVER

            StartupTrace.WriteLine("Protected Startup: Scanning for implementations of IServerConfiguration");

            // Search the application domain for implementations of
            // the IServerConfiguration.
            var serverConfigurations = new List<IServerConfiguration>();
            foreach (var type in typeSource)
            {
                if (typeof(IServerConfiguration).IsAssignableFrom(type) &&
                    !type.IsInterface && !type.IsAbstract)
                {
                    serverConfigurations.Add(Activator.CreateInstance(type) as IServerConfiguration);
                }
            }

            StartupTrace.WriteLine("Protected Startup: Found " + serverConfigurations.Count + " implementations of IServerConfiguration");

            if (gameConfigurations.Count == 0 && serverConfigurations.Count == 0)
            {
                throw new InvalidOperationException(
                    "You must have at least one implementation of " +
                    "IGameConfiguration or IServerConfiguration in your game.");
            }

#else

            if (gameConfigurations.Count == 0)
            {
                throw new InvalidOperationException(
                    "You must have at least one implementation of " +
                    "IGameConfiguration in your game.");
            }

#endif

            StartupTrace.TimingEntries["constructGameConfigurationImplementations"] = stopwatch.Elapsed;
            stopwatch.Restart();

            ICoreGame game = null;

#if CAN_RUN_DEDICATED_SERVER

            ICoreServer server = null;

#endif

            StartupTrace.WriteLine("Protected Startup: Starting iteration through game configuration implementations");

            foreach (var configuration in gameConfigurations)
            {
                StartupTrace.WriteLine("Protected Startup: Configuring kernel with " + configuration.GetType().FullName);
                configuration.ConfigureKernel(kernel);

                StartupTrace.TimingEntries["configureKernel(" + configuration.GetType().FullName + ")"] = stopwatch.Elapsed;
                stopwatch.Restart();

                // We only construct one game.  In the event there are
                // multiple game configurations (such as a third-party library
                // providing additional game tools, it's expected that libraries
                // will return null for ConstructGame).
                if (game == null)
                {
                    StartupTrace.WriteLine("Protected Startup: Attempted to construct game with " + configuration.GetType().FullName);
                    game = configuration.ConstructGame(kernel);
                    StartupTrace.WriteLineIf(game != null, "Protected Startup: Constructed game with " + configuration.GetType().FullName);

                    StartupTrace.TimingEntries["constructGame(" + configuration.GetType().FullName + ")"] = stopwatch.Elapsed;
                    stopwatch.Restart();
                }
            }

            StartupTrace.WriteLine("Protected Startup: Finished iteration through game configuration implementations");

#if CAN_RUN_DEDICATED_SERVER

            StartupTrace.WriteLine("Protected Startup: Starting iteration through server configuration implementations");

            foreach (var serverConfiguration in serverConfigurations)
            {
                StartupTrace.WriteLine("Protected Startup: Configuring kernel with " + serverConfiguration.GetType().FullName);
                serverConfiguration.ConfigureKernel(kernel);

                // We only construct one server.  In the event there are
                // multiple server configurations (such as a third-party library
                // providing additional game tools, it's expected that libraries
                // will return null for ConstructServer).
                if (server == null)
                {
                    StartupTrace.WriteLine("Protected Startup: Attempted to construct server with " + serverConfiguration.GetType().FullName);
                    server = serverConfiguration.ConstructServer(kernel);
                    Debug.WriteLineIf(server != null, "Protected Startup: Constructed server with " + serverConfiguration.GetType().FullName);
                }
            }

            StartupTrace.WriteLine("Protected Startup: Finished iteration through server configuration implementations");

            if (game == null && server == null)
            {
                throw new InvalidOperationException(
                    "No implementation of IGameConfiguration provided " +
                    "a game instance from ConstructGame, and " +
                    "no implementation of IServerConfiguration provided " +
                    "a server instance from ConstructServer.");
            }

            if (game != null && server != null)
            {
                throw new InvalidOperationException(
                    "An implementation of IGameConfiguration provided " +
                    "a game instance, and an implementation of " +
                    "IServerConfiguration provided a server instance.  " +
                    "It's not possible to determine whether the game or " +
                    "server should be started with this configuration.  " +
                    "Move one of the implementations so that it's not in " +
                    "the default loaded application domain.");
            }

#else

            if (game == null)
            {
                throw new InvalidOperationException(
                    "No implementation of IGameConfiguration provided " +
                    "a game instance from ConstructGame.");
            }

#endif

            StartupTrace.TimingEntries["finalizeStartup"] = stopwatch.Elapsed;
            stopwatch.Stop();

            return new StartupSequenceResult
            {
                Kernel = kernel,
                GameInstance = game,
#if CAN_RUN_DEDICATED_SERVER
                ServerInstance = server,
#endif
            };
        }
    }
}