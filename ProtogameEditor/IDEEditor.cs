using System;
using OpenTK.Graphics;
using Ninject;
using Protogame;
using Microsoft.Xna.Framework;
using OpenTK.Platform;

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

        public void OpenWithContext(IGraphicsContext graphicsContext, IWindowInfo windowInfo)
        {
            _kernel = CreateEditorKernel<EditorEmbedContext>();

            var context = _kernel.Get<IEmbedContext>();

            ((EditorEmbedContext)context).GraphicsContext = graphicsContext;
            ((EditorEmbedContext)context).WindowInfo = windowInfo;

            _game = new EditorGame(_kernel);
        }

        public bool Update()
        {
            if (_game != null)
            {
                _game.RunOneFrame();

                return _game.IsActive;
            }
            else
            {
                return true;
            }
        }

        public void Resize(int width, int height)
        {
            if (_kernel == null)
            {
                return;
            }

            Console.WriteLine("resized to " + width + ", " + height);

            var context = (EditorEmbedContext)_kernel.Get<IEmbedContext>();
            context.Width = width;
            context.Height = height;
            context.TriggerResize();
        }
    }
}

