using System;
using Ninject.Extensions.Factory;
using Ninject.Modules;
using Protogame;

namespace MyProject
{
    public class MyProjectModule : NinjectModule
    {
        public override void Load()
        {
            // This was added in the previous tutorial.
            this.Bind<IEntityFactory>().ToFactory();

            // Our new event binder.
            this.Bind<IEventBinder<IGameContext>>().To<MyProjectEventBinder>();
        }
    }
}