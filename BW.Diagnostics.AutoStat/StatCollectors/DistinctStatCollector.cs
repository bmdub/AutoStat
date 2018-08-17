using CardinalityEstimation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

//https://stackoverflow.com/questions/12327004/how-does-the-hyperloglog-algorithm-work
//http://www.moderndescartes.com/essays/hyperloglog/

namespace BW.Diagnostics.StatCollection.Stats
{
    class DistinctStatCollector<T> : IStatCollector<T>
    {
        public string MemberName { get; protected set; }

        ICardinalityEstimator<byte[]> _estimator;
        IByteConverter<T> _byteConverter;

        public DistinctStatCollector(string memberName, Configuration configuration, out bool cancel)
        {
            MemberName = memberName;
            /*if (typeof(T) == typeof(string) ||
                typeof(T) == typeof(int) ||
                typeof(T) == typeof(uint) ||
                typeof(T) == typeof(long) ||
                typeof(T) == typeof(ulong) ||
                typeof(T) == typeof(float) ||
                typeof(T) == typeof(double) ||
                typeof(T) == typeof(byte[]))
            {
                _estimator = (ICardinalityEstimator<T>)new CardinalityEstimator();
            }
            else*/
            {
                _byteConverter = ByteConverterHelpers.GetByteConverter<T>();
                _estimator = new CardinalityEstimator(14, CardinalityEstimation.Hash.HashFunctionId.Fnv1A);
            }

            cancel = false;
        }

        public void AddValue(T value)
        {
            if(value != null)
                _estimator.Add(_byteConverter.ToByteArray(value));
        }

        public double GetHyperLogLog()
        {
            return _estimator.Count();
        }

        public IEnumerable<IStat> GetStats() => new DistinctStat(MemberName, (long)GetHyperLogLog()).ToEnumerable();

        public IEnumerable<IComparedStat> GetStatsComparedTo(IStatCollector statCollector) =>
            GetStats()
                .Zip((statCollector as DistinctStatCollector<T>).GetStats(), (first, second) =>
                    new DistinctComparedStat(MemberName, first as DistinctStat, second as DistinctStat));
    }
}
