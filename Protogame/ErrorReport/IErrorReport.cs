using System;

namespace Protogame
{
    public interface IErrorReport
    {
        void Report(Exception ex, bool exit = true);
    }
}
