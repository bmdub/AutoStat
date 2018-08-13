using System;
using System.Collections.Generic;
using System.Text;

namespace BW.Diagnostics.StatCollection.Stats
{
    /// <summary></summary>
    public static class IComparedStatExtensions
    {
        /// <summary>
        /// Formats a compared stat for display.
        /// </summary>
        /// <param name="comparedStat"></param>
        /// <returns></returns>
        public static string FormatComparedStats(this IComparedStat comparedStat)
        {
            if (comparedStat.Stat1 == null && comparedStat.Stat2 == null)
                return String.Format("({0:P0})", comparedStat.DiffPct);

            if (comparedStat.Stat1.StringValue == null || comparedStat.Stat2.StringValue == null)
                return null;

            //if (comparedStat.IsDifferent == false)
                //return comparedStat.Stat1.StringValue;

            return $"{comparedStat.Stat1.StringValue} / {comparedStat.Stat2.StringValue} {String.Format("({0:P0})", comparedStat.DiffPct)}";
        }
    }
}
