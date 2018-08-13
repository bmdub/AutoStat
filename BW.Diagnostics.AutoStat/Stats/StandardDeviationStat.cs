using System;
using System.Collections.Generic;
using System.Text;

namespace BW.Diagnostics.StatCollection.Stats
{
    /// <summary>
    /// The standard deviation of the member in the set.
    /// </summary>
    public class StandardDeviationStat<T> : IStat
    {
        /// <summary>The name of the class member this is collecting stats on.</summary>
        public string MemberName { get; protected set; }
        /// <summary>The name of the stat.</summary>
        public string Name => "Standard Deviation";
        /// <summary>The string value of the stat.</summary>
        public string StringValue { get; set; }

        /// <summary></summary>
        public double DoubleValue { get; protected set; }
        /// <summary></summary>
        public T Value { get; protected set; }

        internal StandardDeviationStat(string memberName, double doubleValue, T value)
        {
            MemberName = memberName;
            DoubleValue = doubleValue;
            Value = value;
            StringValue = value.ToString();
        }
    }
}
