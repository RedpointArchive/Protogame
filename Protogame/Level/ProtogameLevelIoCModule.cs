using Ninject.Modules;

namespace Protogame
{
    public class ProtogameLevelIoCModule : NinjectModule
    {
        public override void Load()
        {
            this.Bind<ITileset>().To<DefaultTileset>();
            this.Bind<ILevelManager>().To<DefaultLevelManager>();
            this.Bind<ILevelReader>().To<OgmoLevelReader>();
        }
    }
}
