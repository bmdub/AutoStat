using System;
using System.Collections.Generic;
using System.Text;

namespace BW.Diagnostics.StatCollection.Stats
{
    /// <summary>
    /// Comparison of the total non-null and non-default counts of the two stats.
    /// </summary>
    public class NonDefaultCountComparedStat : IComparedStat
    {
        /// <summary>The name of the class member this is collecting stats on.</summary>
        public string MemberName { get; protected set; }
        /// <summary></summary>
        public IStat Stat1 { get; protected set; }
        /// <summary></summary>
        public IStat Stat2 { get; protected set; }
        /// <summary>Indicates whether or not the two stats are different at all.</summary>
        public bool IsDifferent { get; }
        /// <summary>The degree (from -1.0 to 1.0) of difference from stat1 to stat2.</summary>
        public double DiffPct { get; protected set; }

        /// <summary>The name of the stat.</summary>
        public string Name => "Non-Default Value Compared";
        /// <summary>The string value of the stat.</summary>
        public string StringValue { get; set; }

        internal NonDefaultCountComparedStat(string memberName, NonDefaultCountStat stat1, NonDefaultCountStat stat2)
        {
            MemberName = memberName;
            Stat1 = stat1;
            Stat2 = stat2;

            DiffPct = (double)(stat1.CountNonDefault - stat2.CountNonDefault) / stat2.CountNonDefault;
            IsDifferent = DiffPct != 0;

            StringValue = this.FormatComparedStats();
        }
    }
}
