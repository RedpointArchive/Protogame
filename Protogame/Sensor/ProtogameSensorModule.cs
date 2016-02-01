using Protoinject;

namespace Protogame
{
    /// <summary>
    /// This module provides access to hardware sensors on the current
    /// device, such as reading from cameras.
    /// </summary>
    public class ProtogameSensorModule : IProtoinjectModule
    {
        public void Load(IKernel kernel)
        {
            kernel.Bind<ISensorEngine>().To<DefaultSensorEngine>().InSingletonScope();
            kernel.Bind<IEngineHook>().To<SensorEngineHook>();

#if PLATFORM_WINDOWS
            kernel.Bind<ICameraSensor>().To<DirectShowCameraSensor>();
#else
            kernel.Bind<ICameraSensor>().To<NullCameraSensor>();
#endif
        }
    }
}