using System;
using System.Collections.Generic;
using System.Text;

namespace BW.Diagnostics.StatCollection.Stats
{
    /// <summary>
    /// The estimated values at certain percentiles in the set.
    /// </summary>
    public class PercentileStat<T> : IStat
    {
        /// <summary>The name of the class member this is collecting stats on.</summary>
        public string MemberName { get; protected set; }
        /// <summary>The name of the stat.</summary>
        public string Name => $"~ {Percentile * 100.0}th Percentile";
        /// <summary>The string value of the stat.</summary>
        public string StringValue { get; set; }

        /// <summary></summary>
        public double Percentile { get; protected set; }
        /// <summary></summary>
        public T Value { get; protected set; }

        internal PercentileStat(string memberName, double percentile, T value)
        {
            MemberName = memberName;
            Percentile = percentile;
            Value = value;
            StringValue = value.ToStringOrNull();
        }
    }
}
