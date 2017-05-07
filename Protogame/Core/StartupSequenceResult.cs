#if PLATFORM_WINDOWS || PLATFORM_MACOS || PLATFORM_LINUX
#define CAN_RUN_DEDICATED_SERVER
#endif

using Protoinject;

namespace Protogame
{
    public class StartupSequenceResult
    {
        public IKernel Kernel { get; set; }

        public ICoreGame GameInstance { get; set; }

#if CAN_RUN_DEDICATED_SERVER
        public ICoreServer ServerInstance { get; set; }
#endif
    }
}