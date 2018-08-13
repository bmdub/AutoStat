using System;
using System.Collections.Generic;
using System.Linq;

namespace BW.Diagnostics.StatCollection.Stats
{
    class NonDefaultCountStatCollector<T> : IStatCollector<T>
    {
        public string MemberName { get; protected set; }

        long _count;
        long _countNonDefault;
        
        public NonDefaultCountStatCollector(string memberName, Configuration configuration, out bool cancel)
        {
            MemberName = memberName;
            cancel = false;
        }

        public void AddValue(T value)
        {
            _count++;
            if (!EqualityComparer<T>.Default.Equals(value, default))
                _countNonDefault++;
        }

        public IEnumerable<IStat> GetStats()
        {
            if (_count == 0) yield break;
            yield return new NonDefaultCountStat(MemberName, _count, _countNonDefault);
        }

        public IEnumerable<IComparedStat> GetStatsComparedTo(IStatCollector statCollector) =>
            GetStats()
                .Zip((statCollector as NonDefaultCountStatCollector<T>).GetStats(), (first, second) =>
                    new NonDefaultCountComparedStat(MemberName, first as NonDefaultCountStat, second as NonDefaultCountStat));
    }
}
