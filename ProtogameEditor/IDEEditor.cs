using System;
using Protoinject;
using Protogame;
using Microsoft.Xna.Framework;
using System.IO;
using System.Linq;
#if !PLATFORM_WINDOWS
using OpenTK.Graphics;
using OpenTK.Platform;
#endif

namespace ProtogameEditor
{
    public class IDEEditor
    {
        public void Init()
        {
            Console.WriteLine("This is from the editor assembly");
        }

        public string[] GetHandledFileExtensions()
        {
            return new[]
            {
                ".test"
            };
        }

        private EditorGame _game;

        private IKernel _kernel;

        private bool _isSuspended;

        public IKernel CreateEditorKernel<T>() where T : IEmbedContext
        {
            var kernel = new StandardKernel();
            kernel.Load<Protogame3DIoCModule>();
            kernel.Load<ProtogameAssetIoCModule>();
            kernel.Load<ProtogamePlatformingIoCModule>();
            kernel.Load<ProtogameLevelIoCModule>();
            kernel.Load<ProtogameEventsIoCModule>();
            kernel.Bind<ISkin>().To<BasicSkin>();
            kernel.Bind<IBasicSkin>().To<DefaultBasicSkin>();
            kernel.Bind<IEmbedContext>().To<T>().InSingletonScope();
            AssetManagerClient.AcceptArgumentsAndSetup<GameAssetManagerProvider>(kernel, new string[0]);
            return kernel;
        }

        public bool Update()
        {
            if (_game != null && !_isSuspended)
            {
                _game.RunOneFrame();

                return _game.IsActive;
            }
            else
            {
                return true;
            }
        }

        public void Resize(int x, int y, int width, int height)
        {
            if (_kernel == null)
            {
                return;
            }

            Console.WriteLine(x);
            Console.WriteLine(y);

            var context = (EditorEmbedContext)_kernel.Get<IEmbedContext>();
            context.X = x;
            context.Y = y;
            context.Width = width;
            context.Height = height;
            context.TriggerResize();
        }

        public byte[] Suspend()
        {
            _isSuspended = true;
            return null;
        }

		#if PLATFORM_WINDOWS
        public void Resume(IntPtr windowHandle, byte[] state)
		{
			if (windowHandle == IntPtr.Zero) {
				System.Diagnostics.Debug.Write ("Ignoring zero int ptr");
				return;
			}

			System.Diagnostics.Debug.Write ("Resuming with int ptr " + windowHandle);

			_isSuspended = false;

			if (_game == null) {
				_kernel = CreateEditorKernel<EditorEmbedContext> ();

				var context = _kernel.Get<IEmbedContext> ();

				((EditorEmbedContext)context).WindowHandle = windowHandle;

				Console.WriteLine ("Created game with DirectX context.");
				_game = new EditorGame (_kernel);
			} else {
				var context = _kernel.Get<IEmbedContext>();

				((EditorEmbedContext)context).WindowHandle = windowHandle;
			}
		}
		#else
        public void Resume(IGraphicsContext graphicsContext, IWindowInfo windowInfo, byte[] state)
        {
            _isSuspended = false;

            if (_game == null)
            {
                _kernel = CreateEditorKernel<EditorEmbedContext>();

                var context = _kernel.Get<IEmbedContext>();

                ((EditorEmbedContext)context).GraphicsContext = graphicsContext;
                ((EditorEmbedContext)context).WindowInfo = windowInfo;

                _game = new EditorGame(_kernel);
            }
        }
		#endif

        public void Load(string path)
        {
        }

        public void Save(string path)
        {
            using (var writer = new StreamWriter(path))
            {
                foreach (var entity in _game.GameContext.World.GetEntitiesForWorld(_kernel.Hierarchy).OfType<ISerializableEntity>())
                {
                    writer.WriteLine(entity.Text);
                }
            }
        }
    }
}

