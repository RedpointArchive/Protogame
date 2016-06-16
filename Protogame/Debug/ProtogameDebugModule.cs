using Protoinject;

namespace Protogame
{
    public class ProtogameDebugModule : IProtoinjectModule
    {
        public void Load(IKernel kernel)
        {
            kernel.Rebind<IDebugRenderer>().To<DefaultDebugRenderer>().InSingletonScope();
        }
    }
}
