using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Numerics;
using BW.Diagnostics.StatCollection;
using BW.Diagnostics.StatCollection.Stats;

namespace sandbox
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new TestConfiguration(SelectionMode.All, Configuration.DefaultStatCollectors.Append("TestStatCollector"));

            var thingy = new AutoStat<Record>(config);


            Random random = new Random();

            int count = 1_000;
            for (int i = 0; i < count; i++)
            {
                Record record = new Record()
                {
                    Name = "Brian" + i.ToString().PadLeft(6, '0'),
                    Weight = count - (i / (count - i)),
                    Weight2 = i / (count - i),
                    ID = i,
                    TransactionID = Guid.NewGuid(),
                    NetWorth = 5000,
                    SeenDate = DateTime.Now,
                    OtherDate = DateTimeOffset.Now,
                    PocketChange = new decimal(0.01) * new decimal(i),
                    TimeSpent = TimeSpan.FromMinutes(60),
                    CarColor = Color.Red,
                    SomeObject = new object(),
                };

                thingy.Collect(record);
            }

            var recordStats = thingy.GetStats();

            //Console.Write(recordStats.ToTextFormat());
            //recordStats = recordStats.Where(stat => stat.MemberName == "ID").ToRecordStats();
            //Console.Clear();
            Console.Write(recordStats.ToTextTableFormat(Console.WindowWidth));
            //recordStats.OpenCsvInPowershell(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "stats.csv"));


            var thingy3 = new AutoStat<Record>(config);

            for (int i = 0; i < count; i++)
            {
                Record record = new Record()
                {
                    Name = "Brian" + i.ToString().PadLeft(6, '0'),
                    Weight = count - (i / (count - i)),
                    Weight2 = i / (count - i),
                    ID = i + (count / 2),
                    TransactionID = Guid.NewGuid(),
                    NetWorth = 5000,
                    SeenDate = DateTime.Now,
                    OtherDate = DateTimeOffset.Now,
                    PocketChange = new decimal(0.01) * new decimal(i),
                    TimeSpent = TimeSpan.FromMinutes(60),
                    CarColor = Color.Red,
                    SomeObject = new object(),
                };

                thingy3.Collect(record);
            }

            var recordStats2 = thingy3.GetStats();
            //Console.Write(recordStats2.ToTextFormat());
            //Console.Clear();
            Console.Write(recordStats2.ToTextTableFormat(Console.WindowWidth));
            //recordStats2.OpenCsvInPowershell(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "stats.csv"));

            var recordStats3 = thingy.GetStatsComparedTo(thingy3)
                .HighlightWhen(stat => stat.DiffPct >= .30);
            //Console.Write(recordStats3.ToTextFormat());
            //Console.Clear();
            Console.Write(recordStats3.ToTextTableFormat(Console.WindowWidth));
            recordStats3.OpenCsvInPowershell(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "stats.csv"));

            Console.ReadKey();
        }

        class Record
        {
            [AutoStat]
            public string Name { get; set; }
            public int Weight { get; set; }
            public int Weight2 { get; set; }
            public long ID { get; set; }
            public Guid TransactionID { get; set; }
            public BigInteger NetWorth { get; set; }
            public DateTime SeenDate { get; set; }
            public DateTimeOffset OtherDate { get; set; }
            public Decimal PocketChange { get; set; }
            public TimeSpan TimeSpent { get; set; }
            public Color CarColor { get; set; }
            public object SomeObject { get; set; }
            public object SomeNullObject { get; } = null;
            public List<int> SomeList { get; } = new List<int>();
        }

        enum Color
        {
            Red, Green, Blue,
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
}
