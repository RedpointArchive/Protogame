namespace Protogame
{
    using Protoinject;
    
    public class ProtogameProfilingModule : IProtoinjectModule
    {
        public void Load(IKernel kernel)
        {
            kernel.Bind<IProfiler>().To<MemoryProfiler>().InSingletonScope();
            kernel.Bind<IMemoryProfiler>().ToMethod(x => (IMemoryProfiler) x.Kernel.Get<IProfiler>()).InSingletonScope();

            kernel.Bind<IProfilerRenderPass>().To<DefaultProfilerRenderPass>().AllowManyPerScope();

            kernel.Bind<IOperationCostProfilerVisualiser>().To<OperationCostProfilerVisualiser>().InSingletonScope();
        }
    }
}