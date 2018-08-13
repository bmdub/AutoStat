using BW.Diagnostics.StatCollection;
using BW.Diagnostics.StatCollection.Stats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BW.Diagnostics.AutoStat.Test
{
    public class TestStatCollector<T> : IStatCollector<T>
    {
        public string MemberName { get; protected set; }

        long _count;
        int _divisor;

        public TestStatCollector(string memberName, TestConfiguration configuration, out bool cancel)
        {
            MemberName = memberName;
            cancel = false;
            _divisor = configuration.Divisor;
        }

        public void AddValue(T value)
        {
            if (DateTime.UtcNow.Millisecond % 2 == 0)
                _count++;
        }

        public IEnumerable<IStat> GetStats()
        {
            yield return new TestStat(MemberName, _count);
        }

        public IEnumerable<IComparedStat> GetStatsComparedTo(IStatCollector statCollector) =>
            GetStats()
                .Zip((statCollector as TestStatCollector<T>).GetStats(), (first, second) =>
                    new TestComparedStat(MemberName, first as TestStat, second as TestStat));
    }

    public class TestStat : IStat
    {
        public string MemberName { get; protected set; }
        public string Name => "Test";
        public string StringValue { get; set; }

        public long Count { get; protected set; }

        internal TestStat(string memberName, long count)
        {
            MemberName = memberName;
            Count = count;
            StringValue = count.ToString("N0");
        }
    }

    public class TestComparedStat : IComparedStat
    {
        public string MemberName { get; protected set; }
        public IStat Stat1 { get; protected set; }
        public IStat Stat2 { get; protected set; }
        public bool IsDifferent { get; }
        public double DiffPct { get; protected set; }

        public string Name => "Test Compared";
        public string StringValue { get; set; }

        internal TestComparedStat(string memberName, TestStat stat1, TestStat stat2)
        {
            MemberName = memberName;
            Stat1 = stat1;
            Stat2 = stat2;

            DiffPct = (double)(stat1.Count - stat2.Count) / stat2.Count;
            IsDifferent = DiffPct != 0;

            StringValue = this.FormatComparedStats();
        }
    }

    public class TestConfiguration : Configuration
    {
        public int Divisor { get; set; } = 2;

        public TestConfiguration(SelectionMode selectionMode, IEnumerable<string> statCollectorNames)
            : base(selectionMode, statCollectorNames) { }
    }
}
