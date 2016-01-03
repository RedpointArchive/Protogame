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
        
        /*
        var kernel = new StandardKernel();
        kernel.Load<ProtogameCoreModule>();
        kernel.Load<ProtogameAssetIoCModule>();
        kernel.Load<ProtogamePlatformingIoCModule>();
        kernel.Load<ProtogameLevelIoCModule>();
        kernel.Load<ProtogameEventsIoCModule>();
        kernel.Load<TestGame7Module>();
        AssetManagerClient.AcceptArgumentsAndSetup<GameAssetManagerProvider>(kernel, null);

        // Create our OpenGL view, and display it
        var g = new TestGame7Game(kernel);
        SetContentView(g.AndroidGameView);
        g.Run();
        */
    }
}

#endif
