using System;
using System.Collections.Generic;
using System.Linq;

namespace BW.Diagnostics.StatCollection.Stats
{
    class StandardDeviationStatCollector<T> : IStatCollector<T>
    {
        public string MemberName { get; protected set; }

        IMath<T> _math;
        long _count = 0;
        double _mean = 0;
        double _m2 = 0;

        public StandardDeviationStatCollector(string memberName, Configuration configuration, out bool cancel)
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
            var doubleValue = _math.ToDouble(value);
            _count++;
            var delta = doubleValue - _mean;
            _mean += delta / _count;
            _m2 += delta * (doubleValue - _mean);
        }

        public double Get()
        {
            var variance = _m2 / _count;
            return Math.Sqrt(variance);
        }
        
        public IEnumerable<IStat> GetStats()
        {
            if (_count <= 1) yield break;
            
            var standardDeviationDouble = Get();

            var standardDeviation = _math.FromDouble(standardDeviationDouble);
            if (standardDeviation == null) yield break;
            yield return new StandardDeviationStat<T>(MemberName, standardDeviationDouble, standardDeviation);
        }

        public IEnumerable<IComparedStat> GetStatsComparedTo(IStatCollector statCollector) =>
            GetStats()
                .Zip((statCollector as StandardDeviationStatCollector<T>).GetStats(), (first, second) =>
                    new StandardDeviationComparedStat<T>(MemberName, first as StandardDeviationStat<T>, second as StandardDeviationStat<T>));
    }
}
