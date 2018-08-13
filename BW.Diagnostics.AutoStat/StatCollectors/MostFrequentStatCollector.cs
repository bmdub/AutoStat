using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace BW.Diagnostics.StatCollection.Stats
{
    // Uses Sticky Sampling
    class MostFrequentStatCollector<T> : IStatCollector<T>// where T : IComparable
    {
        public string MemberName { get; protected set; }

        MostFrequentStatCollectorConfig _configuration;
        long _count;
        long _nullCount;
        const int _maxValuesToTrack = 1000;
        readonly Dictionary<T, long> _frequencyByValue = new Dictionary<T, long>();
        readonly Random _random = new Random();
        int _countAtSamplingRate;
        int _rateChanges;
        double _initialSampleCount;

        public MostFrequentStatCollector(string memberName, Configuration configuration, out bool cancel)
        {
            MemberName = memberName;
            _configuration = configuration.MostFrequentStatCollectorConfig;

            const double supportThreshold = 1;
            const double errorProbability = 0.1;
            const double failureProbability = 0.01;
            _initialSampleCount = (1.0 / errorProbability) * Math.Log(1 / (failureProbability * supportThreshold));

            if (typeof(T).GetMethod("ToString", new Type[0]).DeclaringType == typeof(object))
                cancel = true;
            else
                cancel = false;
        }

        public void AddValue(T value)
        {
            _count++;

            // Gradually decrease the sampling rate
            bool rateChange = false;
            if (_countAtSamplingRate == 0)
            {
                _countAtSamplingRate = (int)((2 << _rateChanges) * _initialSampleCount);
                _rateChanges++;
                rateChange = true;
            }
            _countAtSamplingRate--;

            // Trim the counts at every rate change
            if (rateChange)
            {
                foreach (var pair in _frequencyByValue.ToList())
                {
                    while (_random.Next(2) == 0)
                    {
                        long newCount = pair.Value - 1;
                        if (newCount > 0)
                            _frequencyByValue[pair.Key] = newCount;
                        else
                            _frequencyByValue.Remove(pair.Key);
                    }
                }
            }

            // Trim the counts if we are collecting too much data
            while (_frequencyByValue.Count > _maxValuesToTrack)
            {
                foreach (var pair in _frequencyByValue.ToList())
                {
                    long newCount = pair.Value - 1;
                    if (newCount > 0)
                        _frequencyByValue[pair.Key] = newCount;
                    else
                        _frequencyByValue.Remove(pair.Key);
                }
            }

            if (value == null)
                _nullCount++;
            else if (_frequencyByValue.TryGetValue(value, out long count))
                _frequencyByValue[value] = count + 1;
            else if (_random.NextDouble() <= 1.0 / _rateChanges)
                _frequencyByValue[value] = 1;
        }

        public IEnumerable<IStat> GetStats()
        {
            if (_frequencyByValue.Count == 0 && _nullCount == 0) yield break;

            // Get the values by frequency
            IEnumerable<KeyValuePair<T, long>> entries = _frequencyByValue;

            // Include any null values
            if (_nullCount > 0)
                entries = entries.Concat(new KeyValuePair<T, long>(default(T), _nullCount).ToEnumerable());

            int n = 0;
            foreach (var entry in entries.OrderByDescending(entry => entry.Value).Take(_configuration.Count))
            {
                n++;
                yield return new MostFrequentStat<T>(MemberName, n, entry.Key);
            }
        }

        public IEnumerable<IComparedStat> GetStatsComparedTo(IStatCollector statCollector)
        {
            yield break;
        }
    }

}
