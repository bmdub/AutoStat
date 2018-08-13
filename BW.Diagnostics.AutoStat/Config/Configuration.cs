using BW.Diagnostics.StatCollection.Stats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace BW.Diagnostics.StatCollection
{
    /// <summary>
    /// Configuration for an AutoStat instance.
    /// </summary>
    public partial class Configuration
    {
        /// <summary>The default list of stat collector names used.</summary>
        public static readonly string[] DefaultStatCollectors = new string[]
        {
            "CountStatCollector",
            "DistinctStatCollector",
            "MeanStatCollector",
            "MinMaxStatCollector",
            "MostFrequentStatCollector",
            "NonDefaultCountStatCollector",
            "PercentileStatCollector",
            "SampleStatCollector",
            "StandardDeviationStatCollector",
            "SumStatCollector",
        };

        /// <summary>The configuration for the PercentileStatCollector.</summary>
        public PercentileStatCollectorConfig PercentileStatCollectorConfig = new PercentileStatCollectorConfig();
        /// <summary>The configuration for the MostFrequentStatCollector.</summary>
        public MostFrequentStatCollectorConfig MostFrequentStatCollectorConfig = new MostFrequentStatCollectorConfig();
        /// <summary>The stat collectors configured for use.</summary>
        public IEnumerable<Type> StatCollectorTypes { get; private set; }
        /// <summary>The selection mode to use when looking for properties to collect stats on.</summary>
        public SelectionMode SelectionMode { get; }

        /// <summary>Configuration for an AutoStat instance.</summary>
        /// <param name="selectionMode">The selection mode to use when looking for properties to collect stats on.</param>
        /// <param name="statCollectorNames">The names of the stat collector types to use for collection.</param>
        public Configuration(SelectionMode selectionMode, params string[] statCollectorNames)
        {
            SelectionMode = selectionMode;

            if (statCollectorNames.Length == 0)
                statCollectorNames = DefaultStatCollectors;

            // Look for matching stat collectors in all loaded assemblies.
            var potentialStatCollectors = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => typeof(IStatCollector).IsAssignableFrom(type))
                .Where(type => !type.IsAbstract)
                .ToList();

            List<Type> collectorTypes = new List<Type>();

            foreach (var statCollectorName in statCollectorNames)
            {
                var collector = potentialStatCollectors.Where(type => type.Name.Contains(statCollectorName)).FirstOrDefault();

                if (collector == null) throw new TypeLoadException($"Type matching the name '{statCollectorName}' not found.");

                collectorTypes.Add(collector);
            }

            StatCollectorTypes = collectorTypes;
        }

        /// <summary>Configuration for an AutoStat instance.</summary>
        /// <param name="selectionMode">The selection mode to use when looking for properties to collect stats on.</param>
        /// <param name="statCollectorNames">The names of the stat collector types to use for collection.</param>
        public Configuration(SelectionMode selectionMode, IEnumerable<string> statCollectorNames)
            : this(selectionMode, statCollectorNames.ToArray())
        {
        }
    }
}
