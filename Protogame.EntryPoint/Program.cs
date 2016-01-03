#if PLATFORM_WINDOWS || PLATFORM_MACOS || PLATFORM_LINUX || PLATFORM_WEB || PLATFORM_IOS

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Protogame;
using Protoinject;

public static class Program
{
    public static void Main(string[] args)
    {
        var kernel = new StandardKernel();
        
        Func<System.Reflection.Assembly, Type[]> tryGetTypes = assembly =>
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

        // Search the application domain for implementations of
        // the IGameConfiguration.
        var configurations =
            (from assembly in AppDomain.CurrentDomain.GetAssemblies()
             from type in tryGetTypes(assembly)
             where typeof(IGameConfiguration).IsAssignableFrom(type) &&
                   !type.IsInterface && !type.IsAbstract
             select Activator.CreateInstance(type) as IGameConfiguration).ToList();

        if (configurations.Count == 0)
        {
            throw new InvalidOperationException(
                "You must have at least one implementation of " +
                "IGameConfiguration in your game.");
        }

        Game game = null;

        foreach (var configuration in configurations)
        {
            configuration.ConfigureKernel(kernel);

            // It is expected that the AssetManagerProvider architecture will
            // be refactored in future to just provide IAssetManager directly,
            // and this method call will be dropped.
            configuration.InitializeAssetManagerProvider(new AssetManagerProviderInitializer(kernel, args));

            // We only construct one game.  In the event there are
            // multiple game configurations (such as a third-party library
            // providing additional game tools, it's expected that libraries
            // will return null for ConstructGame).
            if (game == null)
            {
                game = configuration.ConstructGame(kernel);
            }
        }

        if (game == null)
        {
            throw new InvalidOperationException(
                "No implementation of IGameConfiguration provided " +
                "returned a game instance from ConstructGame.");
        }

        using (var runningGame = game)
        {
            runningGame.Run();
        }
    }
}

#endif
