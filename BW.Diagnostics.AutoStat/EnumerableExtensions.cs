using BW.Diagnostics.StatCollection.Stats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BW.Diagnostics.StatCollection
{
    /// <summary></summary>
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Creates a RecordStats object from a list of stats.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stats"></param>
        /// <returns></returns>
        public static RecordStats<T> ToRecordStats<T>(this IEnumerable<T> stats) where T : IStat => new RecordStats<T>(stats);
    }
}
