#if PLATFORM_ANDROID || PLATFORM_OUYA

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Content.PM;

using Microsoft.Xna.Framework;

using Protoinject;

using Protogame;

[Activity(
    Label = "Game",
    MainLauncher = true,
    AlwaysRetainTaskState = true,
    LaunchMode = Android.Content.PM.LaunchMode.SingleInstance,
    ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.Keyboard | ConfigChanges.KeyboardHidden,
    ScreenOrientation = ScreenOrientation.Landscape)]
public class GameActivity : AndroidGameActivity
{
    protected override void OnCreate(Bundle bundle)
    {
        base.OnCreate(bundle);

        var kernel = new StandardKernel();

        // Search the application domain for implementations of
        // the IGameConfiguration.
        var configurations =
            (from assembly in AppDomain.CurrentDomain.GetAssemblies()
             from type in assembly.GetTypes()
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
            configuration.InitializeAssetManagerProvider(new AssetManagerProviderInitializer(kernel, new string[0]));

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
        
        SetContentView(((ICoreGame)game).AndroidGameView);
        game.Run();
    }
}

#endif
