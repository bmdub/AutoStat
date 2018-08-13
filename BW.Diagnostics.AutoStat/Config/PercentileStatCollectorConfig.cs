namespace BW.Diagnostics.StatCollection
{
    /// <summary></summary>
    public class PercentileStatCollectorConfig
    {
        /// <summary>
        /// The percentiles to return values for.
        /// </summary>
        public double[] Percentiles { get; set; } = new double[] { .001, .01, .05, .95, .99, .999 };
    }
}
