using System;
using System.Collections.Generic;
using System.Linq;

namespace BW.Diagnostics.StatCollection.Stats
{
    class SumStatCollector<T> : IStatCollector<T>
    {
        public string MemberName { get; protected set; }

        IMath<T> _math;
        double _totalValue;

        public SumStatCollector(string memberName, Configuration configuration, out bool cancel)
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
        }

        public IEnumerable<IStat> GetStats()
        {
            T sum = _math.FromDouble(_totalValue);
            if (sum == null) yield break;
            yield return new SumStat<T>(MemberName, sum);
        }

        public IEnumerable<IComparedStat> GetStatsComparedTo(IStatCollector statCollector) =>
            GetStats()
                .Zip((statCollector as SumStatCollector<T>).GetStats(), (first, second) =>
                    new SumComparedStat<T>(MemberName, first as SumStat<T>, second as SumStat<T>));
    }
}
