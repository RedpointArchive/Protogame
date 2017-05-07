using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Protogame
{
    public static class StartupTrace
    {
        static StartupTrace()
        {
            TimingEntries = new Dictionary<string, TimeSpan>();
        }

        public static Dictionary<string, TimeSpan> TimingEntries { get; }

        public static bool EmittedTimingEntries { get; set; }

        public static bool EmitStartupTrace = false;

        [Conditional("DEBUG")]
        public static void WriteLine(string message)
        {
            if (EmitStartupTrace)
            {
                Debug.WriteLine(message);
            }
        }

        [Conditional("DEBUG")]
        public static void WriteLineIf(bool condition, string message)
        {
            if (EmitStartupTrace)
            {
                Debug.WriteLineIf(condition, message);
            }
        }
    }
}