using System;
using System.Collections.Generic;
using System.Text;

namespace BW.Diagnostics.StatCollection.Stats
{
    static class IStatExtensions
    {
        public static StatCollectorKey GetKey(this IStatCollector statCollector) => new StatCollectorKey(statCollector.MemberName, statCollector.GetType().Name);
    }
}
