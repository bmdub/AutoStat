using System;
using System.Collections.Generic;
using System.Text;

namespace BW.Diagnostics.StatCollection
{
    /// <summary>
    /// Indicates whether or not to collect stats for the decorated member.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public class AutoStatAttribute : Attribute
    {
    }
}
