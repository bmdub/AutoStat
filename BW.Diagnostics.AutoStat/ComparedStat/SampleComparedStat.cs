using System;
using System.Collections.Generic;
using System.Text;

namespace BW.Diagnostics.StatCollection.Stats
{
    /// <summary>
    /// Comparison of the values of the two sets.
    /// </summary>
    public class SampleComparedStat<T> : IComparedStat
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
        public string Name => "~ Sample Compared";
        /// <summary>The string value of the stat.</summary>
        public string StringValue { get; set; }

        internal SampleComparedStat(string memberName, IList<(ulong hash, T value)> hashes1, IList<(ulong hash, T value)> hashes2)
        {
            MemberName = memberName;
            
            Dictionary<ulong, T> hashes2Set = new Dictionary<ulong, T>();
            foreach (var kvp in hashes2) hashes2Set[kvp.hash] = kvp.value;

            int mismatchCount = 0;

            foreach (var hash in hashes1)
            {
                if (hashes2Set.TryGetValue(hash.hash, out T value) == false)
                {
                    mismatchCount++;
                    continue;
                }

                if (EqualityComparer<T>.Default.Equals(value, hash.value) == false)
                    mismatchCount++;
            }
            
            DiffPct = (double)mismatchCount / hashes1.Count;
            if (double.IsNaN(DiffPct))
                if (hashes2.Count > 0) DiffPct = 1;
                else DiffPct = 0;
            IsDifferent = DiffPct != 0;

            StringValue = this.FormatComparedStats();
        }
    }
}
