using System;
using System.Collections.Generic;
using System.Text;

namespace BW.Diagnostics.StatCollection.Stats
{
    /// <summary>
    /// The estimated cardinality of the member in the set.
    /// </summary>
    public class DistinctStat : IStat
    {
        /// <summary>The name of the class member this is collecting stats on.</summary>
        public string MemberName { get; protected set; }
        /// <summary>The name of the stat.</summary>
        public string Name => "~ Distinct";
        /// <summary>The string value of the stat.</summary>
        public string StringValue { get; set; }

        /// <summary></summary>
        public long Count { get; protected set; }

        internal DistinctStat(string memberName, long count)
        {
            MemberName = memberName;
            Count = count;
            StringValue = count.ToString("N0");
        }
    }
}
