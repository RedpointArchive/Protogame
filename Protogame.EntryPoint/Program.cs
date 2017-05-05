#if PLATFORM_WINDOWS || PLATFORM_MACOS || PLATFORM_LINUX || PLATFORM_WEB || PLATFORM_IOS

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
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
#endif

#if PLATFORM_MACOS

public static class Program
{
	public static void Main(string[] args)
	{
        if (args.Contains("--debug-startup"))
        {
            StartupTrace.EmitStartupTrace = true;
        }

		NSApplication.Init();

		StartupTrace.WriteLine("Startup: Finished NSApplication.Init in static Main");

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
    private static IKernel _kernel;
    private static Game _game;
    private static ICoreServer _server;

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

		StartupTrace.WriteLine("Startup: Reached AppDelegate.DidFinishLaunching / AppDelegate.FinishedLaunching");

		ErrorProtection.RunEarly(() => ProtectedStartup(args));
		ErrorProtection.RunMain(_kernel.TryGet<IErrorReport>(), ProtectedRun);
	}
#elif PLATFORM_IOS
using Foundation;
using UIKit;

[Register("AppDelegate")]
public class Program : UIApplicationDelegate
{
    private static IKernel _kernel;
    private static Game _game;
    private static ICoreServer _server;

    public static void Main(string[] args)
    {
        if (args.Contains("--debug-startup"))
        {
            StartupTrace.EmitStartupTrace = true;
        }

        UIApplication.Main(args, null, "AppDelegate");
    }

    public override void FinishedLaunching(UIApplication app)
    {
        var args = new string[0];

		ErrorProtection.RunEarly(() => ProtectedStartup(args));
		ErrorProtection.RunMain(_kernel.TryGet<IErrorReport>(), ProtectedRun);
    }
#else
public static class Program
{
    private static IKernel _kernel;
    private static HostGame _game;
    private static ICoreServer _server;

    public static void Main(string[] args)
    {
        if (args.Contains("--debug-startup"))
        {
            StartupTrace.EmitStartupTrace = true;
        }

        ErrorProtection.RunEarly(() => ProtectedStartup(args));
        ErrorProtection.RunMain(_kernel.TryGet<IErrorReport>(), ProtectedRun);
    }
#endif

    private static void ProtectedStartup(string[] args)
    {
        var result = StartupSequence.Start(args);
        _kernel = result.Kernel;
        _game = new HostGame(result.GameInstance);
#if PLATFORM_WINDOWS || PLATFORM_MACOS || PLATFORM_LINUX
        _server = result.ServerInstance;
#endif
    }

    private static void ProtectedRun()
    {
#if PLATFORM_WINDOWS || PLATFORM_MACOS || PLATFORM_LINUX
        if (_game != null)
        {
#endif
            StartupTrace.WriteLine("Protected Run: Starting game");

#if PLATFORM_MACOS || PLATFORM_IOS
			_game.Run();
#else
        	using (var runningGame = _game)
        	{
            	runningGame.Run();
        	}
#endif

#if PLATFORM_WINDOWS || PLATFORM_MACOS || PLATFORM_LINUX
        }
        else if (_server != null)
        {
            StartupTrace.WriteLine("Protected Run: Starting server");

			_server.Run();
        }
#endif
    }
}

#endif
