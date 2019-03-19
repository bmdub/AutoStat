using System;
using System.Collections.Generic;
using System.Text;

namespace BW.Diagnostics.StatCollection.Stats
{
    /// <summary>
    /// Comparison of the values of the two sets.
    /// </summary>
    public class ExistenceComparedStat<T> : IComparedStat
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
        public string Name => "~ Existence Compared";
        /// <summary>The string value of the stat.</summary>
        public string StringValue { get; set; }

        internal ExistenceComparedStat(string memberName, IList<ulong> hashes1, IList<ulong> hashes2)
        {
            MemberName = memberName;
            HashSet<ulong> hashes1Set = new HashSet<ulong>(hashes1);
            HashSet<ulong> hashes2Set = new HashSet<ulong>(hashes2);

            int nonExistCount = 0;
            foreach (var hash in hashes1)
                if (!hashes2Set.Contains(hash)) nonExistCount++;
            foreach (var hash in hashes2)
                if (!hashes1Set.Contains(hash)) nonExistCount++;

            DiffPct = (double)nonExistCount / (hashes1.Count + hashes2.Count);
            IsDifferent = DiffPct != 0;

            StringValue = this.FormatComparedStats();
        }
    }
}
