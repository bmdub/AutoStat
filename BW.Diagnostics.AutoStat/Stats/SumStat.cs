using System;
using System.Collections.Generic;
using System.Text;

namespace BW.Diagnostics.StatCollection.Stats
{
    /// <summary>
    /// The sum of all values of the member in the set.
    /// </summary>
    public class SumStat<T> : IStat
    {
        /// <summary>The name of the class member this is collecting stats on.</summary>
        public string MemberName { get; protected set; }
        /// <summary>The name of the stat.</summary>
        public string Name => "Sum";
        /// <summary>The string value of the stat.</summary>
        public string StringValue { get; set; }

        /// <summary></summary>
        public T Sum { get; protected set; }

        internal SumStat(string memberName, T sum)
        {
            MemberName = memberName;
            Sum = sum;
            StringValue = sum.ToString();
        }
    }
}
