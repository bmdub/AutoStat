using System;
using System.Collections.Generic;
using System.Linq;

namespace BW.Diagnostics.StatCollection.Stats
{
    class MeanStatCollector<T> : IStatCollector<T>
    {
        public string MemberName { get; protected set; }

        IMath<T> _math;
        double _totalValue;
        long _count;

        public MeanStatCollector(string memberName, Configuration configuration, out bool cancel)
        {
            MemberName = memberName;

            _math = MathHelpers.GetMath<T>();
            if (_math == null)
                cancel = true;
            else
                cancel = false;
        }

        public void AddValue(ulong keyHash, T value)
        {
            _totalValue += _math.ToDouble(value);
            _count++;
        }
        
        public IEnumerable<IStat> GetStats()
        {
            if (_count == 0) yield break;
            var mean = _math.FromDouble(_totalValue / _count);
            if (mean == null) yield break;
            yield return new MeanStat<T>(MemberName, _count == 0 ? default : mean);
        }

        public IEnumerable<IComparedStat> GetStatsComparedTo(IStatCollector statCollector) =>
            GetStats()
                .Zip((statCollector as MeanStatCollector<T>).GetStats(), (first, second) =>
                    new MeanComparedStat<T>(MemberName, first as MeanStat<T>, second as MeanStat<T>));
    }
}
