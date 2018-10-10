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
            Dictionary<ulong, T> hashesSet = new Dictionary<ulong, T>();
            foreach (var kvp in hashes2) hashesSet.Add(kvp.hash, kvp.value);

            int mismatchCount = 0;
            foreach (var hash in hashes1)
            {
                if (hashesSet.TryGetValue(hash.hash, out T value) == false)
                {
                    mismatchCount++;
                    continue;
                }

                if (EqualityComparer<T>.Default.Equals(value, hash.value) == false)
                    mismatchCount++;
            }

            var max = hashes2.Count;// Math.Max(hashesSet.Count, otherHashes.Count);
            if (max == 0) max = 1;

            DiffPct = (double)mismatchCount / max;
            IsDifferent = DiffPct != 0;

            StringValue = this.FormatComparedStats();
        }
    }
}
