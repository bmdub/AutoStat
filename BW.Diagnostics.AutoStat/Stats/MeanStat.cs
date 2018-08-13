using System;
using System.Collections.Generic;
using System.Text;

namespace BW.Diagnostics.StatCollection.Stats
{
    /// <summary>
    /// The average value of the member in the set.
    /// </summary>
    public class MeanStat<T> : IStat
    {
        /// <summary>The name of the class member this is collecting stats on.</summary>
        public string MemberName { get; protected set; }
        /// <summary>The name of the stat.</summary>
        public string Name => "Mean";
        /// <summary>The string value of the stat.</summary>
        public string StringValue { get; set; }

        /// <summary></summary>
        public T Mean { get; protected set; }

        internal MeanStat(string memberName, T mean)
        {
            MemberName = memberName;
            Mean = mean;
            StringValue = mean.ToString();
        }
    }
}
