using System;
using System.Diagnostics;

namespace Protogame
{
    public static class ErrorProtection
    {
        public static void RunEarly(Action entryPoint)
        {
            if (Debugger.IsAttached)
            {
                entryPoint();
                return;
            }

            try
            {
                entryPoint();
            }
            catch (Exception ex)
            {
                // Very early on in game startup, we don't have the kernel fully initialized yet,
                // so we have to rely on builtin implementations without DI.
                IErrorReportOutput errorReportOutput;

#if PLATFORM_WINDOWS
                errorReportOutput = new WindowsFormsErrorReportOutput();
#elif PLATFORM_MACOS || PLATFORM_LINUX
                errorReportOutput = new ConsoleErrorReportOutput();
#else
                errorReportOutput = new NullErrorReportOutput();
#endif

                var errorReportFallback = new DefaultErrorReport(errorReportOutput);
                errorReportFallback.Report(ex);
            }
        }

        public static void RunMain(IErrorReport errorReport, Action entryPoint)
        {
            if (errorReport == null || Debugger.IsAttached)
            {
                entryPoint();
                return;
            }

            try
            {
                entryPoint();
            }
            catch (Exception ex)
            {
                errorReport.Report(ex);
            }
        }
    }
}
