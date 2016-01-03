#if PLATFORM_WINDOWS || PLATFORM_MACOS || PLATFORM_LINUX || PLATFORM_WEB || PLATFORM_IOS

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Protogame;
using Protoinject;

#if PLATFORM_MACOS
#if PLATFORM_MACOS_LEGACY
using MonoMac.AppKit;
using MonoMac.Foundation;
#else
using AppKit;
using Foundation;
#endif

public static class Program
{
	public static void Main(string[] args)
	{
		NSApplication.Init();

		using (var p = new NSAutoreleasePool())
		{
			NSApplication.SharedApplication.Delegate = new AppDelegate();

			// TODO: Offer a way of setting the application icon.
			//NSImage appIcon = NSImage.ImageNamed("monogameicon.png");
			//NSApplication.SharedApplication.ApplicationIconImage = appIcon;

			NSApplication.Main(args);
		}
	}
}

public class AppDelegate : NSApplicationDelegate
{
	public override bool ApplicationShouldTerminateAfterLastWindowClosed(NSApplication sender)
	{
		return true;
	}

#if PLATFORM_MACOS_LEGACY
	public override void FinishedLaunching(NSObject notification)
#else
	public override void DidFinishLaunching(NSNotification notification)
#endif
	{
		var args = new string[0];
#else
public static class Program
{
    public static void Main(string[] args)
    {
#endif
        var kernel = new StandardKernel();
        
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

        // Search the application domain for implementations of
        // the IGameConfiguration.
        var configurations =
            (from assembly in AppDomain.CurrentDomain.GetAssemblies()
				from type in TryGetTypes(assembly)
            where typeof (IGameConfiguration).IsAssignableFrom(type) &&
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
			
#if PLATFORM_MACOS
		game.Run();
#else
        using (var runningGame = game)
        {
            runningGame.Run();
        }
#endif
    }
}

#endif
