using System;
using System.Collections.Generic;
using System.Text;

namespace BW.Diagnostics.StatCollection.Stats
{
    /// <summary>
    /// Defines a single statistic for a given member of a class.
    /// </summary>
    public interface IStat
    {
        /// <summary>The name of the class member this is collecting stats on.</summary>
        string MemberName { get; }
        /// <summary>The name of the stat.</summary>
        string Name { get; }
        /// <summary>The string value of the stat.</summary>
        string StringValue { get; set; }
    }
}
