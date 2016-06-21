using Protoinject;

namespace Protogame
{
    public class ProtogameErrorReportModule : IProtoinjectModule
    {
        public void Load(IKernel kernel)
        {
            kernel.Bind<IErrorReport>().To<DefaultErrorReport>().InSingletonScope();

#if PLATFORM_WINDOWS
            kernel.Bind<IErrorReportOutput>().To<WindowsFormsErrorReportOutput>().InSingletonScope();
#elif PLATFORM_MACOS || PLATFORM_LINUX
            kernel.Bind<IErrorReportOutput>().To<ConsoleErrorReportOutput>().InSingletonScope();
#else
            kernel.Bind<IErrorReportOutput>().To<NullErrorReportOutput>().InSingletonScope();
#endif
        }
    }
}
