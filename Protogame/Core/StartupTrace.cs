using System;
using System.Collections.Generic;

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
    }
}