using Ninject.Modules;

namespace Protogame
{
    public class ContentCompilationIoCModule : NinjectModule
    {
        public override void Load()
        {
            #if PLATFORM_WINDOWS || PLATFORM_MAC || PLATFORM_LINUX
            this.Bind<IContentCompiler>().To<DefaultContentCompiler>();
            #else
            this.Bind<IContentCompiler>().To<NullContentCompiler>();
            #endif
        }
    }
}
