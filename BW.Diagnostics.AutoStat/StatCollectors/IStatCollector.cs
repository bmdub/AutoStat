
using System.Collections.Generic;

namespace BW.Diagnostics.StatCollection.Stats
{
    /// <summary>
    /// Defines a stat collector that collects and returns statistics.
    /// </summary>
    public interface IStatCollector
    {
        /// <summary>The name of the class member this is collecting stats on.</summary>
        string MemberName { get; }
        /// <summary>Calculates and returns statistics for this member.</summary>
        IEnumerable<IStat> GetStats();
        /// <summary>Calculates and returns comparison statistics as compared to another stat collector for the same member.</summary>
        IEnumerable<IComparedStat> GetStatsComparedTo(IStatCollector statCollector);
    }

    /// <summary>
    /// Defines a stat collector that collects and returns statistics.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IStatCollector<T> : IStatCollector
    {
        /// <summary>
        /// Accepts a record instance for the collection of statistics.
        /// </summary>
        /// <param name="keyHash"></param>
        /// <param name="value"></param>
        void AddValue(ulong keyHash, T value);
    }
}
