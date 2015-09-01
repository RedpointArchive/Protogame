using Ninject.Extensions.Factory;
using Ninject.Modules;

namespace Protogame
{
    public class ProtogameComponentModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IBuiltinComponentFactory>().ToFactory();
        }
    }
}