namespace BW.Diagnostics.StatCollection
{
    /// <summary>
    /// Defines what members to collect stats for.
    /// </summary>
    public enum SelectionMode
    {
        /// <summary>
        /// Collect stats on all public properties.
        /// </summary>
        All = 0,

        /// <summary>
        /// Collect stats only on public properties decorated with the AutoStatAttribute.
        /// </summary>
        Attribute = 1,
    }
}
