using System;

namespace Protogame
{
    public class NullErrorReportOutput : IErrorReportOutput
    {
        public void Report(Exception ex)
        {
        }
    }
}