using Protoinject;

namespace Protogame
{
    public class ProtogameImageProcessingModule : IProtoinjectModule
    {
        public void Load(IKernel kernel)
        {
            kernel.Bind<IImageProcessingFactory>().ToFactory();
            kernel.Bind<IColorInImageDetection>().To<ThreadedColorInImageDetection>();
            kernel.Bind<IColorInImageDetectionDebugRenderer>()
                .To<DefaultColorInImageDetectionDebugRenderer>()
                .InSingletonScope();
        }
    }
}