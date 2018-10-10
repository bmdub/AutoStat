using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Numerics;
using BW.Diagnostics.StatCollection;
using BW.Diagnostics.StatCollection.Stats;
using System.Diagnostics;

namespace sandbox
{
    class Program
    {
        static void Main(string[] args)
        {
            //var config = new TestConfiguration(SelectionMode.Attribute, Configuration.DefaultStatCollectors.Append("TestStatCollector"));
            var config = new TestConfiguration(SelectionMode.All, Configuration.DefaultStatCollectors.Append("TestStatCollector"));

            //var autoStat1 = new AutoStat<Host>(config);
            var autoStat1 = new AutoStat<Host>(keyName: "id");
            //var autoStat2 = new AutoStat<Host>(config);
            var autoStat2 = new AutoStat<Host>(keyName: "id");

            Random random = new Random();
            int recordCount = 1_000_000;

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            for (int i = 0; i < recordCount; i++)
            {
                Host record = new Host()
                {
                    Name = "Wyatt" + i.ToString().PadLeft(6, '0'),
                    SerialNumber = i,
                    Id = i,
                    Uptime = TimeSpan.FromMinutes(i),
                    PokerMoney = new decimal(0.01) * new decimal(i),
                    DeathCount = recordCount - (i / (recordCount - i)),
                    Awareness = Awareness.NotAlive,
                    Escaped = random.Next(0, 2) == 1
                };

                autoStat1.Collect(record);
            }
            Console.WriteLine(stopwatch.Elapsed);
            //return;
            var recordStats1 = autoStat1.GetStats();

            Console.Write(recordStats1.ToTextFormat());
            //recordStats = recordStats.Where(stat => stat.MemberName == "SerialNumber").ToRecordStats();
            Console.Write(recordStats1.ToTextTableFormat(Console.WindowWidth));
            //recordStats1.OpenCsvInPowershell("stats.csv");
            

            for (int i = 0; i < recordCount; i++)
            {
                Host record = new Host()
                {
                    Name = "Wyatt" + i.ToString().PadLeft(6, '0'),
                    SerialNumber = i + recordCount,
                    Id = i,
                    Uptime = TimeSpan.FromMinutes(i),
                    PokerMoney = new decimal(0.02) * new decimal(i),
                    DeathCount = recordCount - (i / (recordCount - i)),
                    Awareness = random.Next(0, 2) == 1 ? Awareness.NotAlive : Awareness.Alive,
                    Escaped = true
                };

                autoStat2.Collect(record);
            }

            var recordStats2 = autoStat2.GetStats();

            Console.Write(recordStats2.ToTextFormat());
            Console.Write(recordStats2.ToTextTableFormat(Console.WindowWidth));
            //recordStats1.OpenCsvInPowershell("stats.csv");

            var recordStats3 = autoStat1.GetStatsComparedTo(autoStat2)
                .HighlightWhen(stat => stat.DiffPct >= .30);

            Console.Write(recordStats3.ToTextFormat());
            Console.Write(recordStats3.ToTextTableFormat(Console.WindowWidth));
            //recordStats1.OpenCsvInPowershell("stats.csv");

            Console.ReadKey();
        }

        class Host
        {
            public string Name { get; set; }
            [AutoStat]
            public long SerialNumber { get; set; }
            public long Id { get; set; }
            public TimeSpan Uptime { get; set; }
            public Decimal PokerMoney { get; set; }
            public int DeathCount { get; set; }
            public Awareness Awareness { get; set; }
            public bool Escaped { get; set; }
        }

        enum Awareness
        {
            NotAlive, Alive, AliveAndPissed,
        }

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

            public void AddValue(ulong keyHash, T value)
            {
                // Collect our stat here for this record
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
}
