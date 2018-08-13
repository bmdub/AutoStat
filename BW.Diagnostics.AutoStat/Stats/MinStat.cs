using System;
using System.Collections.Generic;
using System.Text;

namespace BW.Diagnostics.StatCollection.Stats
{
    /// <summary>
    /// The minimum value of the member in the set.
    /// </summary>
    public class MinStat<T> : IStat
    {
        /// <summary>The name of the class member this is collecting stats on.</summary>
        public string MemberName { get; protected set; }
        /// <summary>The name of the stat.</summary>
        public string Name => "Min";
        /// <summary>The string value of the stat.</summary>
        public string StringValue { get; set; }

        /// <summary></summary>
        public T Min { get; protected set; }

        internal MinStat(string memberName, T min)
        {
            MemberName = memberName;
            Min = min;
            StringValue = min.ToString();
        }
    }
}
