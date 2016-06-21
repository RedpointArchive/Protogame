#if PLATFORM_WINDOWS || PLATFORM_MACOS || PLATFORM_LINUX

using System;

namespace Protogame
{
    public class ConsoleErrorReportOutput : IErrorReportOutput
    {
        public void Report(Exception ex)
        {
            Console.WriteLine(ex);
        }
    }
}

#endif