using System;
using System.Collections.Generic;
using System.Text;

namespace BW.Diagnostics.StatCollection.Stats
{
    /// <summary>
    /// Defines a class that's used for comparing 2 given stats.
    /// </summary>
    public interface IComparedStat : IStat
    {
        /// <summary></summary>
        IStat Stat1 { get; }
        /// <summary></summary>
        IStat Stat2 { get; }

        /// <summary>Indicates whether or not the two stats are different at all.</summary>
        bool IsDifferent { get; }
        /// <summary>The degree (from -1.0 to 1.0) of difference from stat1 to stat2.</summary>
        double DiffPct { get; }
    }
}
