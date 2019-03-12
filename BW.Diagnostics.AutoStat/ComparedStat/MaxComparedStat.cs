using System;
using System.Collections.Generic;
using System.Text;

namespace BW.Diagnostics.StatCollection.Stats
{
    /// <summary>
    /// Comparison of the max value of the two stats.
    /// </summary>
    public class MaxComparedStat<T> : IComparedStat
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
        public string Name => "Max Compared";
        /// <summary>The string value of the stat.</summary>
        public string StringValue { get; set; }

        internal MaxComparedStat(string memberName, MaxStat<T> stat1, MaxStat<T> stat2)
        {
            MemberName = memberName;
            Stat1 = stat1;
            Stat2 = stat2;

            var equal = EqualityComparer<T>.Default.Equals(stat1.Max, stat2.Max);

            IsDifferent = !equal;

            var math = MathHelpers.GetMath<T>();
            if (math != null)
            {
                var val1 = math.ToDouble(stat1.Max);
                var val2 = math.ToDouble(stat2.Max);
                DiffPct = (val2 - val1) / val1;
                if (double.IsNaN(DiffPct)) DiffPct = 0;
            }
            else
            {
                DiffPct = equal ? 0.0 : 1.0;
            }            

            StringValue = this.FormatComparedStats();
        }
    }
}
