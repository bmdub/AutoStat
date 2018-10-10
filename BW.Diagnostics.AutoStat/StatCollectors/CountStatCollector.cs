
using System;
using System.Collections.Generic;
using System.Linq;

namespace BW.Diagnostics.StatCollection.Stats
{
    class CountStatCollector<T> : IStatCollector<T>
    {
        public string MemberName { get; protected set; }

        long _count;

        public CountStatCollector(string memberName, Configuration configuration, out bool cancel)
        {
            MemberName = memberName;
            cancel = false;
        }

        public void AddValue(ulong keyHash, T value) => _count++;

        public IEnumerable<IStat> GetStats() => new CountStat(MemberName, _count).ToEnumerable();

        public IEnumerable<IComparedStat> GetStatsComparedTo(IStatCollector statCollector) =>
            GetStats()
                .Zip((statCollector as CountStatCollector<T>).GetStats(), (first, second) =>
                    new CountComparedStat(MemberName, first as CountStat, second as CountStat));
    }
}
