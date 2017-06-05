using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Threading;
#if PLATFORM_LINUX
using System;
#endif

namespace Protogame
{
    public class HostGame : Game, IHostGame
    {
        private GraphicsDeviceManager _graphicsDeviceManager;
        private SpriteBatch _splashSpriteBatch;
        private Texture2D _splash;
        private Color _backgroundColor;
#if PLATFORM_ANDROID
        private AndroidGameActivity _gameActivity;
#endif
        private ICoreGame _coreGame;
        private ICoreGame _pendingCoreGame;
        private bool _hasStartedDelayLoad;
        private bool _shouldLoadContentOnDelayedGame;

        /// <summary>
        /// Whether resize events can be handled.
        /// </summary>
        private bool _shouldHandleResize;

        /// <summary>
        /// The current known window width, this is used to detect window maximize / minimize, which
        /// doesn't seem to fire OnClientSizeChanged.
        /// </summary>
        private int _windowWidth;

        /// <summary>
        /// The current known window height, this is used to detect window maximize / minimize, which
        /// doesn't seem to fire OnClientSizeChanged.
        /// </summary>
        private int _windowHeight;

#if PLATFORM_ANDROID
        public HostGame(AndroidGameActivity gameActivity)
#else
        public HostGame(ICoreGame preloadedCoreGame)
#endif
        {
#if !PLATFORM_ANDROID
            _coreGame = preloadedCoreGame;
#endif

            _graphicsDeviceManager = new GraphicsDeviceManager(this);

            if (_coreGame != null)
            {
                _coreGame.AssignHost(this);
                _shouldHandleResize = true;
                _windowWidth = Window.ClientBounds.Width;
                _windowHeight = Window.ClientBounds.Height;
                _coreGame.PrepareGraphicsDeviceManager(_graphicsDeviceManager);
                _graphicsDeviceManager.PreparingDeviceSettings +=
                    (sender, e) =>
                    {
                        _coreGame.PrepareDeviceSettings(e.GraphicsDeviceInformation);
                    };
            }
            else
            {
                _shouldLoadContentOnDelayedGame = false;
                _backgroundColor = Color.Black;
#if PLATFORM_ANDROID
                _graphicsDeviceManager.IsFullScreen = true;
                _gameActivity = gameActivity;
                try
                {
                    var layout = _gameActivity.Window.DecorView.FindViewById(Android.Resource.Id.Content);
                    var colorId = _gameActivity.Resources.GetIdentifier("splash_background", "color", Android.App.Application.Context.PackageName);
                    var androidColor = _gameActivity.Resources.GetColor(colorId);
                    _backgroundColor = new Color(androidColor.R, androidColor.G, androidColor.B, (byte)255);
                }
                catch
                {
                    // Setting background is optional.
                }
#endif
            }
        }

        /// <summary>
        /// A platform independent representation of a game window.
        /// </summary>
        /// <value>
        /// The game window.
        /// </value>
        public IGameWindow ProtogameWindow { get; private set; }

        /// <summary>
        /// The graphics device manager used by the game.
        /// </summary>
        /// <value>
        /// The graphics device manager.
        /// </value>
        public GraphicsDeviceManager GraphicsDeviceManager => _graphicsDeviceManager;

        public SpriteBatch SplashScreenSpriteBatch => _splashSpriteBatch;

        public Texture2D SplashScreenTexture => _splash;

        protected override void LoadContent()
        {
            GraphicsDevice.DeviceLost += (sender, e) =>
            {
                _coreGame?.DeviceLost();
            };
            GraphicsDevice.DeviceReset += (sender, e) =>
            {
                _coreGame?.DeviceReset();
            };
            GraphicsDevice.DeviceResetting += (sender, e) =>
            {
                _coreGame?.DeviceResetting();
            };
            GraphicsDevice.ResourceCreated += (sender, e) =>
            {
                _coreGame?.ResourceCreated(e.Resource);
            };
            GraphicsDevice.ResourceDestroyed += (sender, e) =>
            {
                _coreGame?.ResourceDestroyed(e.Name, e.Tag);
            };

            // Construct a platform-independent game window.
            ProtogameWindow = ConstructGameWindow();

#if PLATFORM_ANDROID
            // On Android, disable viewport / backbuffer scaling because we expect games
            // to make use of the full display area.
            this.GraphicsDeviceManager.IsFullScreen = true;
            this.GraphicsDeviceManager.PreferredBackBufferHeight = this.Window.ClientBounds.Height;
            this.GraphicsDeviceManager.PreferredBackBufferWidth = this.Window.ClientBounds.Width;
#endif

#if PLATFORM_WINDOWS
            // Register for the window resize event so we can scale
            // the window correctly.
            Window.ClientSizeChanged += OnClientSizeChanged;

            // Register for the window close event so we can dispatch
            // it correctly.
            var form = System.Windows.Forms.Control.FromHandle(Window.Handle) as System.Windows.Forms.Form;
            if (form != null)
            {
                form.FormClosing += (sender, args) =>
                {
                    bool cancel = false;
                    _coreGame?.CloseRequested(out cancel);

                    if (cancel)
                    {
                        args.Cancel = true;
                    }
                };
            }
#endif

            if (_coreGame != null)
            {
                _coreGame.LoadContent();
                return;
            }

            _shouldLoadContentOnDelayedGame = true;

            _splashSpriteBatch = new SpriteBatch(GraphicsDevice);
            
#if PLATFORM_ANDROID
            try
            {
                var layout = _gameActivity.Window.DecorView.FindViewById(Android.Resource.Id.Content);
                var backgroundId = _gameActivity.Resources.GetIdentifier("background", "drawable", Android.App.Application.Context.PackageName);
                _splash = Texture2D.FromStream(GraphicsDevice, Android.Graphics.BitmapFactory.DecodeResource(_gameActivity.Resources, backgroundId));
            }
            catch
            {
                // Setting background is optional.
            }
#endif
        }

        protected override void UnloadContent()
        {
            if (_coreGame != null)
            {
                _coreGame.UnloadContent();
                return;
            }

             _shouldLoadContentOnDelayedGame = false;
        }

        protected override void Update(GameTime gameTime)
        {
            if (_pendingCoreGame != null && _coreGame == null)
            {
                _coreGame = _pendingCoreGame;
                _coreGame.AssignHost(this);

                if (_shouldLoadContentOnDelayedGame)
                {
                    _coreGame.LoadContent();
                }
            }

            if (_coreGame != null)
            {
#if PLATFORM_WINDOWS
                if (Window != null)
                {
                    if (_windowWidth != Window.ClientBounds.Width ||
                        _windowHeight != Window.ClientBounds.Height)
                    {
                        OnClientSizeChanged(this, EventArgs.Empty);
                    }
                }
#endif

                _coreGame.Update(gameTime);
                return;
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            if (_coreGame != null)
            {
                _coreGame.Draw(gameTime);
                return;
            }

            GraphicsDevice.Clear(Color.Black);

            var width = GraphicsDevice.PresentationParameters.BackBufferWidth;
            var height = GraphicsDevice.PresentationParameters.BackBufferHeight;

            _splashSpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            var rect = new Rectangle(0, 0, width, height);
            if (_splash != null)
            {
                _splashSpriteBatch.Draw(
                    _splash,
                    rect,
                    Color.White);
            }
            _splashSpriteBatch.End();
            
            if (!_hasStartedDelayLoad && _coreGame == null)
            {
                ThreadPool.QueueUserWorkItem(o =>
                {
                    var result = StartupSequence.Start(new string[0]);
                    _pendingCoreGame = result.GameInstance;
                });
                _hasStartedDelayLoad = true;
            }
        }

        private void OnClientSizeChanged(object sender, EventArgs args)
        {
            if (!_shouldHandleResize)
            {
                return;
            }

            _shouldHandleResize = false;
            _windowWidth = Window.ClientBounds.Width;
            _windowHeight = Window.ClientBounds.Height;
            _graphicsDeviceManager.PreferredBackBufferWidth = _windowWidth;
            _graphicsDeviceManager.PreferredBackBufferHeight = _windowHeight;
            _graphicsDeviceManager.ApplyChanges();
            _shouldHandleResize = true;
        }

        protected override void Dispose(bool disposing)
        {
            if (_coreGame != null)
            {
                _coreGame.Dispose(disposing);
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Constructs an implementation of <see cref="IGameWindow"/> based on the current game.  This method
        /// abstracts the current platform.
        /// </summary>
        /// <returns>
        /// The game window instance.
        /// </returns>
        private IGameWindow ConstructGameWindow()
        {
#if PLATFORM_WINDOWS || PLATFORM_MACOS || PLATFORM_LINUX || PLATFORM_WEB || PLATFORM_IOS
            return new DefaultGameWindow(this, base.Window);
#elif PLATFORM_ANDROID || PLATFORM_OUYA
            return new AndroidGameWindow((Microsoft.Xna.Framework.AndroidGameWindow)base.Window);
#endif
        }

        /// <summary>
        /// Runs code before MonoGame performs any initialization logic.
        /// </summary>
        // ReSharper disable once EmptyConstructor
        static HostGame()
        {
#if PLATFORM_LINUX
            LoadPrimusRunPathForDualGPUDevices();
#endif
        }

#if PLATFORM_LINUX
        public static void LoadPrimusRunPathForDualGPUDevices()
        {
            const string primusRunPath = "/usr/bin/primusrun";
            var basePath = new System.IO.FileInfo(System.Reflection.Assembly.GetEntryAssembly().Location).DirectoryName;
            if (System.IO.File.Exists(primusRunPath))
            {
                // primusrun exists, we should try and upgrade
                // the graphics before libGL is loaded so that we
                // can use the NVIDIA GPU instead of Intel (which
                // generally doesn't work with the render pipeline
                // on Linux).
                Console.Error.WriteLine(
                    "Detected Linux system with primusrun; will attempt to use NVIDIA GPU!");
                var process = new System.Diagnostics.Process();
                process.StartInfo = new System.Diagnostics.ProcessStartInfo
                {
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    FileName = primusRunPath,
                    Arguments = "env",
                    WorkingDirectory = basePath
                };
                process.Start();
                using (var output = process.StandardOutput)
                {
                    var regex = new System.Text.RegularExpressions.Regex(
                        "^LD_LIBRARY_PATH=(.*)$",
                        System.Text.RegularExpressions.RegexOptions.Multiline);
                    var m = regex.Match(output.ReadToEnd());
                    if (m.Success)
                    {
                        var ldLibraryPath = m.Groups[1].Value;

                        Console.Error.WriteLine("Creating symbolic links to NVIDIA libGL...");
                        var created = new System.Collections.Generic.List<string>();
                        foreach (var path in ldLibraryPath.Split(':'))
                        {
                            var dir = new System.IO.DirectoryInfo(path);
                            if (dir.Exists)
                            {
                                foreach (var f in dir.GetFiles())
                                {
                                    if (!created.Contains(f.Name) && !System.IO.File.Exists(System.IO.Path.Combine(basePath, f.Name)))
                                    {
                                        Console.Error.WriteLine("Mapping " + f.Name + " to " + f.FullName + "...");
                                        var ln = new System.Diagnostics.Process();
                                        ln.StartInfo = new System.Diagnostics.ProcessStartInfo
                                        {
                                            UseShellExecute = false,
                                            FileName = "/usr/bin/ln",
                                            Arguments = "-s '" + f.FullName + "' '" + System.IO.Path.Combine(basePath, f.Name) + "'",
                                            WorkingDirectory = basePath
                                        };
                                        ln.Start();
                                        ln.WaitForExit();
                                        created.Add(f.Name);
                                    }
                                }
                            }
                        }
                        Console.Error.WriteLine("Created symbolic links so that NVIDIA GPU is used.");
                    }
                    else
                    {
                        Console.Error.WriteLine(
                            "Unable to find newer LD_LIBRARY_PATH, rendering might not work correctly!");
                    }
                }
            }
        }
#endif
    }
}