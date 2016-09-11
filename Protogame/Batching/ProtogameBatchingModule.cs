// ReSharper disable CheckNamespace
#pragma warning disable 1591

using Protoinject;

namespace Protogame
{
    /// <summary>
    /// The Protogame batching module allows you to have physics objects and renderable components
    /// automatically combined into more efficient batches.
    /// </summary>
    public class ProtogameBatchingModule : IProtoinjectModule
    {
        public void Load(IKernel kernel)
        {
            kernel.Bind<IStaticBatching>().To<DefaultStaticBatching>().InSingletonScope();
            kernel.Bind<IBatchedControlFactory>().ToFactory();
        }
    }
}
