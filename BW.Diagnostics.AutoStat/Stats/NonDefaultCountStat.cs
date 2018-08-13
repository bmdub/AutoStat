using System;
using System.Collections.Generic;
using System.Text;

namespace BW.Diagnostics.StatCollection.Stats
{
    /// <summary>
    /// The count of non-null / non-default values in the set.
    /// </summary>
    public class NonDefaultCountStat : IStat
    {
        /// <summary>The name of the class member this is collecting stats on.</summary>
        public string MemberName { get; protected set; }
        /// <summary>The name of the stat.</summary>
        public string Name => "Non-Default Count";
        /// <summary>The string value of the stat.</summary>
        public string StringValue { get; set; }

        /// <summary></summary>
        public long Count { get; protected set; }
        /// <summary></summary>
        public long CountNonDefault { get; protected set; }
        /// <summary></summary>
        public double PctNonDefault { get; protected set; }

        internal NonDefaultCountStat(string memberName, long count, long countNonDefault)
        {
            MemberName = memberName;
            Count = count;
            CountNonDefault = countNonDefault;
            StringValue = countNonDefault.ToString("N0");
        }
    }
}

