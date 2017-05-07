#if PLATFORM_ANDROID || PLATFORM_OUYA

using Android.App;
using Android.OS;
using Android.Views;
using Android.Content.PM;

using Microsoft.Xna.Framework;

using Protogame;

#if DEBUG
[assembly: Application(Debuggable = true)]
#else
[assembly: Application(Debuggable = false)]
#endif

[Activity(
    Label = "@string/app_name",
    Icon = "@drawable/icon",
    Theme = "@style/Protogame.Splash",
    Immersive = true,
    MainLauncher = true,
    AlwaysRetainTaskState = true,
    LaunchMode = LaunchMode.SingleInstance,
    ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.Keyboard | ConfigChanges.KeyboardHidden,
    ScreenOrientation = ScreenOrientation.Landscape)]
public class GameActivity : AndroidGameActivity
{
    private HostGame _hostGame;
    private bool _hasInitedGame;

    protected override async void OnCreate(Bundle bundle)
    {
        base.OnCreate(bundle);
        HideSystemUi();

        _hostGame = new HostGame(this);
        var view = (View)_hostGame.Services.GetService(typeof(View));
        SetContentView(view);
        _hostGame.Run();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        _hostGame.Dispose();
    }

    protected override void OnResume()
    {
        base.OnResume();
        HideSystemUi();
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
