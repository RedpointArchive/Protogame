using System;

namespace Protogame
{
    public interface IErrorReportOutput
    {
        void Report(Exception ex);
    }
}
