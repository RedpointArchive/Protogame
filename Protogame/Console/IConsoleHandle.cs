using System;

namespace Protogame
{
    public interface IConsoleHandle
    {
        [Obsolete("Use one of the other Log methods on the console handle instead.")]
        void Log(string messageFormat);

        [Obsolete("Use one of the other Log methods on the console handle instead.")]
        void Log(string messageFormat, params object[] objects);

        void LogDebug(string messageFormat);

        void LogDebug(string messageFormat, params object[] objects);

        void LogInfo(string messageFormat);

        void LogInfo(string messageFormat, params object[] objects);

        void LogWarning(string messageFormat);

        void LogWarning(string messageFormat, params object[] objects);

        void LogError(string messageFormat);

        void LogError(string messageFormat, params object[] objects);

        void LogError(Exception exception);
    }
}
