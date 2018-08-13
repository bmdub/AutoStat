using System;
using System.Collections.Generic;
using System.Text;

namespace BW.Diagnostics.StatCollection.Stats
{
    /// <summary>
    /// The maxmimum value of the member in the set.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MaxStat<T> : IStat
    {
        /// <summary>The name of the class member this is collecting stats on.</summary>
        public string MemberName { get; protected set; }
        /// <summary>The name of the stat.</summary>
        public string Name => "Max";
        /// <summary>The string value of the stat.</summary>
        public string StringValue { get; set; }

        /// <summary></summary>
        public T Max { get; protected set; }

        internal MaxStat(string memberName, T max)
        {
            MemberName = memberName;
            Max = max;
            StringValue = max.ToString();
        }
    }
}
