﻿using System;
using System.Collections.Generic;
using System.Text;

namespace BW.Diagnostics.StatCollection.Stats
{
    /// <summary>
    /// Comparison of the standard deviation of the two stats.
    /// </summary>
    public class StandardDeviationComparedStat<T> : IComparedStat
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
        public string Name => "Standard Deviation Compared";
        /// <summary>The string value of the stat.</summary>
        public string StringValue { get; set; }

        internal StandardDeviationComparedStat(string memberName, StandardDeviationStat<T> stat1, StandardDeviationStat<T> stat2)
        {
            MemberName = memberName;
            Stat1 = stat1;
            Stat2 = stat2;

            var math = MathHelpers.GetMath<T>();

            var standardDeviation1 = math.ToDouble(stat1.Value);
            var standardDeviation2 = math.ToDouble(stat2.Value);

            DiffPct = (standardDeviation2 - standardDeviation1) / standardDeviation1;
            if (double.IsNaN(DiffPct)) DiffPct = 0;
            IsDifferent = DiffPct != 0;

            StringValue = this.FormatComparedStats();
        }
    }
}
