﻿using System;
using System.Collections.Generic;
using System.Text;

namespace BW.Diagnostics.StatCollection.Stats
{
    /// <summary>
    /// Comparison of the sum of the two stats.
    /// </summary>
    public class SumComparedStat<T> : IComparedStat
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
        public string Name => "Sum Compared";
        /// <summary>The string value of the stat.</summary>
        public string StringValue { get; set; }

        internal SumComparedStat(string memberName, SumStat<T> stat1, SumStat<T> stat2)
        {
            MemberName = memberName;
            Stat1 = stat1;
            Stat2 = stat2;

            var math = MathHelpers.GetMath<T>();

            var sum1 = math.ToDouble(stat1.Sum);
            var sum2 = math.ToDouble(stat2.Sum);

            DiffPct = (sum2 - sum1) / sum1;
            if (double.IsNaN(DiffPct)) DiffPct = 0;
            IsDifferent = DiffPct != 0;

            StringValue = this.FormatComparedStats();
        }
    }
}
