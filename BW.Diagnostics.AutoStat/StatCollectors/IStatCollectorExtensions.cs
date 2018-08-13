using System;
using System.Collections.Generic;
using System.Text;

namespace BW.Diagnostics.StatCollection.Stats
{
    static class IStatCollectorExtensions
    {
        public static StatKey GetKey(this IStat stat) => new StatKey(stat.MemberName, stat.Name);
    }
}
