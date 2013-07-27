using Ninject.Modules;

namespace Protogame
{
    public class ProtogameIoCModule : NinjectModule
    {
        public override void Load()
        {
            this.Bind<IRenderUtilities>().To<DefaultRenderUtilities>();
            this.Bind<IAudioUtilities>().To<DefaultAudioUtilities>();
            this.Bind<IGameContext>().To<DefaultGameContext>();
            this.Bind<IUpdateContext>().To<DefaultUpdateContext>();
            this.Bind<IRenderContext>().To<DefaultRenderContext>();
        }
    }
}

