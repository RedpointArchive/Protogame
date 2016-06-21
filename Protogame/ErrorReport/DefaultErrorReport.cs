using System;

namespace Protogame
{
    public class DefaultErrorReport : IErrorReport
    {
        private readonly IErrorReportOutput _errorReportOutput;

        public DefaultErrorReport(IErrorReportOutput errorReportOutput)
        {
            _errorReportOutput = errorReportOutput;
        }

        public void Report(Exception ex, bool exit = true)
        {
            _errorReportOutput.Report(ex);

            if (exit)
            {
                Environment.Exit(1);
            }
        }
    }
}
