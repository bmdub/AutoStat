using System;
using System.Collections.Generic;
using System.Text;

namespace BW.Diagnostics.StatCollection.Stats
{
    /// <summary>
    /// The most frequent values of the member in the set.
    /// </summary>
    public class MostFrequentStat<T> : IStat
    {
        /// <summary>The name of the class member this is collecting stats on.</summary>
        public string MemberName { get; protected set; }
        /// <summary>The name of the stat.</summary>
        public string Name { get; protected set; }
        /// <summary>The string value of the stat.</summary>
        public string StringValue { get; set; }

        /// <summary></summary>
        public int N { get; protected set; }
        /// <summary></summary>
        public T Value { get; protected set; }
        
        internal MostFrequentStat(string memberName, int n, T value)
        {
            MemberName = memberName;

            N = n;
            if (n == 1) Name = "~ 1st Most Frequent";
            else if (n == 2) Name = "~ 2nd Most Frequent";
            else if (n == 3) Name = "~ 3rd Most Frequent";
            else Name = $"~ {n}th Most Frequent";

            Value = value;
            StringValue = value.ToStringOrNull();
        }
    }
}
