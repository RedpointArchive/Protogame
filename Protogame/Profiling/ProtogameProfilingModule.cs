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
            kernel.Bind<INetworkTrafficProfilerVisualiser>().To<NetworkTrafficProfilerVisualiser>().InSingletonScope();
            kernel.Bind<IGraphicsMetricsProfilerVisualiser>().To<GraphicsMetricsProfilerVisualiser>().InSingletonScope();
            kernel.Bind<IPhysicsMetricsProfilerVisualiser>().To<PhysicsMetricsProfilerVisualiser>().InSingletonScope();
            kernel.Bind<IKernelMetricsProfilerVisualiser>().To<KernelMetricsProfilerVisualiser>().InSingletonScope();
            kernel.Bind<IGCMetricsProfilerVisualiser>().To<GCMetricsProfilerVisualiser>().InSingletonScope();
        }
    }
}