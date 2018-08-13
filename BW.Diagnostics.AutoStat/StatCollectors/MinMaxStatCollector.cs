using System;
using System.Collections.Generic;
using System.Linq;

namespace BW.Diagnostics.StatCollection.Stats
{
    class MinMaxStatCollector<T> : IStatCollector<T> where T : IComparable<T>
    {
        public string MemberName { get; protected set; }

        IMath<T> _math;
        T _minValue;
        T _maxValue;

        public MinMaxStatCollector(string memberName, Configuration configuration, out bool cancel)
        {
            MemberName = memberName;
            cancel = false;
            _math = MathHelpers.GetMath<T>();
        }

        public void AddValue(T value)
        {
            if (value?.CompareTo(_maxValue) > 0)
                _maxValue = value;
            else if (value?.CompareTo(_minValue) < 0)
                _minValue = value;
        }

        public IEnumerable<IStat> GetStats()
        {
            if (_minValue != null)
                yield return new MinStat<T>(MemberName, _minValue);
            if (_maxValue != null)
                yield return new MaxStat<T>(MemberName, _maxValue);
        }

        public IEnumerable<IComparedStat> GetStatsComparedTo(IStatCollector statCollector)
        {
            return GetStats()
                .Zip((statCollector as MinMaxStatCollector<T>).GetStats(), (first, second) =>
                {
                    if (first is MinStat<T>)
                        return (IComparedStat)(new MinComparedStat<T>(MemberName, first as MinStat<T>, second as MinStat<T>));
                    else
                        return (IComparedStat)(new MaxComparedStat<T>(MemberName, first as MaxStat<T>, second as MaxStat<T>));
                });
        }
    }
}
