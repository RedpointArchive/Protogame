#if PLATFORM_ANDROID || PLATFORM_OUYA

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Android.App;
using Android.OS;
using Android.Views;
using Android.Content.PM;

using Microsoft.Xna.Framework;

using Protoinject;

using Protogame;
using System.Diagnostics;

[Activity(
    Label = "@string/app_name",
    Icon = "@drawable/icon",
    Theme = "@android:style/Theme.NoTitleBar",
    MainLauncher = true,
    AlwaysRetainTaskState = true,
    LaunchMode = LaunchMode.SingleInstance,
    ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.Keyboard | ConfigChanges.KeyboardHidden,
    ScreenOrientation = ScreenOrientation.Landscape)]
public class GameActivity : AndroidGameActivity
{
    protected override void OnCreate(Bundle bundle)
    {
        base.OnCreate(bundle);

        var stopwatch = new Stopwatch();
        stopwatch.Start();

        var kernel = new StandardKernel();
        kernel.Bind<IRawLaunchArguments>()
            .ToMethod(x => new DefaultRawLaunchArguments(new string[0]))
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

        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        var typeSource = new List<Type>();
        foreach (var assembly in assemblies)
        {
            typeSource.AddRange(assembly.GetCustomAttributes<ConfigurationAttribute>().Select(x => x.GameConfigurationOrServerClass));
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

        if (gameConfigurations.Count == 0)
        {
            throw new InvalidOperationException(
                "You must have at least one implementation of " +
                "IGameConfiguration in your game.");
        }
        
        StartupTrace.TimingEntries["constructGameConfigurationImplementations"] = stopwatch.Elapsed;
        stopwatch.Restart();

        Game game = null;

        foreach (var configuration in gameConfigurations)
        {
            configuration.ConfigureKernel(kernel);

            StartupTrace.TimingEntries["configureKernel(" + configuration.GetType().FullName + ")"] = stopwatch.Elapsed;
            stopwatch.Restart();

            // We only construct one game.  In the event there are
            // multiple game configurations (such as a third-party library
            // providing additional game tools, it's expected that libraries
            // will return null for ConstructGame).
            if (game == null)
            {
                game = configuration.ConstructGame(kernel);

                StartupTrace.TimingEntries["constructGame(" + configuration.GetType().FullName + ")"] = stopwatch.Elapsed;
                stopwatch.Restart();
            }
        }

        if (game == null)
        {
            throw new InvalidOperationException(
                "No implementation of IGameConfiguration provided " +
                "returned a game instance from ConstructGame.");
        }
        
        SetContentView(((ICoreGame)game).AndroidGameView);

        game.Run();

        ((ICoreGame)game).AndroidGameView.RequestFocus();
        
        StartupTrace.TimingEntries["finalizeStartup"] = stopwatch.Elapsed;
        stopwatch.Stop();
    }

    public override void OnWindowFocusChanged(bool hasFocus)
    {
        base.OnWindowFocusChanged(hasFocus);

        if (hasFocus)
        {
            HideSystemUi();
        }
    }

    private void HideSystemUi()
    {
        Window.DecorView.SystemUiVisibility =
            (StatusBarVisibility)(SystemUiFlags.LayoutStable | SystemUiFlags.LayoutHideNavigation | SystemUiFlags.LayoutFullscreen | SystemUiFlags.HideNavigation | SystemUiFlags.Fullscreen | SystemUiFlags.ImmersiveSticky);
        
    }
}

#endif
