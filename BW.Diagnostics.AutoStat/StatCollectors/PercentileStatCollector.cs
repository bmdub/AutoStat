using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace BW.Diagnostics.StatCollection.Stats
{
    //Reservoir sampling
    class PercentileStatCollector<T> : IStatCollector<T> where T : IComparable<T>
    {
        public string MemberName { get; protected set; }

        PercentileStatCollectorConfig _configuration;
        const double MaxSamples = 10_000;
        long _count = 0;
        readonly List<T> _items = new List<T>((int)MaxSamples);
        readonly Random _random = new Random();

        public PercentileStatCollector(string memberName, Configuration configuration, out bool cancel)
        {
            MemberName = memberName;
            _configuration = configuration.PercentileStatCollectorConfig;
            cancel = false;
        }

        public void AddValue(ulong keyHash, T value)
        {
            _count++;
            if (_items.Count < MaxSamples)
                _items.Add(value);
            else if (_random.NextDouble() <= MaxSamples / _count)
                _items[_random.Next(_items.Count)] = value;
        }

        public IEnumerable<IStat> GetStats()
        {
            if (_items.Count == 0) yield break;

            _items.Sort();

            foreach (var percentile in _configuration.Percentiles)
                yield return new PercentileStat<T>(MemberName, percentile, _items[(int)((_items.Count - 1) * percentile)]);
        }

        public IEnumerable<IComparedStat> GetStatsComparedTo(IStatCollector statCollector)
        {
            yield break;
        }
    }
}
