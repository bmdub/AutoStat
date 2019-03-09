using BW.Diagnostics.StatCollection;
using BW.Diagnostics.StatCollection.Stats;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace BW.Diagnostics.AutoStat.Test
{
    public class UnitTest1
    {
        [Fact]
        public void NullTest()
        {
            var autostat = new AutoStat<TestRecord>();

            autostat.GetStats().ToTextFormat();
            autostat.GetStats().ToTextTableFormat(20);
        }

        [Fact]
        public void CustomStatTest()
        {
            var config = new TestConfiguration(SelectionMode.All, Configuration.DefaultStatCollectors.Append("TestStatCollector"));

            var autostat = new AutoStat<TestRecord>(config);

            autostat.GetStats().ToTextFormat();
        }

        [Fact]
        public void CustomMemberTest()
        {
            var config = new Configuration(SelectionMode.Attribute);

            var autostat = new AutoStat<TestRecord>(config);

            autostat.Collect(new TestRecord());

            var stats = autostat.GetStats().ToList();

            Assert.True(stats.Where(stat => stat.MemberName != "Name").Any() == false, "More than the specified member had statistics.");
        }

        [Fact]
        public void CustomStatSelectionTest()
        {
            var config = new Configuration(SelectionMode.All, "CountStatCollector");

            var autostat = new AutoStat<TestRecord>(config);

            autostat.Collect(new TestRecord());

            var stats = autostat.GetStats().ToList();

            Assert.True(stats.Where(stat => stat.Name != "Count").Any() == false, "More than the specified stat has been collected.");
        }

        [Fact]
        public void StatsTest()
        {
            var autostat = new AutoStat<TestRecord>();
            
            int count = 1_000;
            for (int i = 0; i < count; i++)
            {
                TestRecord record = new TestRecord()
                {
                    Name = "John" + i.ToString().PadLeft(6, '0'),
                    Weight = count - (i / (count - i)),
                    ID = i,
                    TransactionID = Guid.NewGuid(),
                    NetWorth = 5000,
                    SeenDate = DateTime.Now,
                    OtherDate = DateTimeOffset.Now,
                    PocketChange = new decimal(0.01) * new decimal(i),
                    TimeSpent = TimeSpan.FromMinutes(60),
                    CarColor = Color.Red,
                    SomeObject = new object(),
                    SometimesNullObject = i % 2 == 0 ? null : new object()
                };

                autostat.Collect(record);
            }

            var recordStats = autostat.GetStats();

            Assert.True((recordStats.Where(stat => stat.MemberName == "Name" && stat.Name == "Count").First() as CountStat).Count == 1_000);
            Assert.True((recordStats.Where(stat => stat.MemberName == "SometimesNullObject" && stat.Name == "Non-Default Count").First() as NonDefaultCountStat).CountNonDefault == 500);
            Assert.True(Math.Abs((recordStats.Where(stat => stat.MemberName == "Name" && stat.Name.Contains("Distinct")).First() as DistinctStat).Count - 1_000) < 100);
            Assert.True((recordStats.Where(stat => stat.MemberName == "Weight" && stat.Name == "Max").First() as MaxStat<int>).Max == 1_000);
            Assert.True((recordStats.Where(stat => stat.MemberName == "Weight" && stat.Name == "Min").First() as MinStat<int>).Min == 0);
            Assert.True(Math.Abs((recordStats.Where(stat => stat.MemberName == "ID" && stat.Name == "Mean").First() as MeanStat<long>).Mean - 500) <= 1);
            Assert.True(Math.Abs((recordStats.Where(stat => stat.MemberName == "ID" && stat.Name == "Standard Deviation").First() as StandardDeviationStat<long>).Value - 288) <= 1);
            //Assert.True((recordStats.Where(stat => stat.MemberName == "ID" && stat.Name == "Sum").First() as SumStat<long>).Sum == 499500);

            Assert.True((recordStats.Where(stat => stat.MemberName == "Weight" && stat.Name == "~ 1th Percentile").First() as PercentileStat<int>).Value == 901);
            Assert.True((recordStats.Where(stat => stat.MemberName == "Weight" && stat.Name.Contains("1st Most Frequent")).First() as MostFrequentStat<int>).Value == 1000);
        }

        [Fact]
        public void ComparedStatsWithKeyTest()
        {
            var autostat = new AutoStat<TestRecord>(keyName: "id");
            
            int count = 1_000;
            for (int i = 0; i < count; i++)
            {
                TestRecord record = new TestRecord()
                {
                    Name = "John" + i.ToString().PadLeft(6, '0'),
                    Weight = count - (i / (count - i)),
                    ID = i,
                    TransactionID = Guid.NewGuid(),
                    NetWorth = 5000,
                    SeenDate = DateTime.Now,
                    OtherDate = DateTimeOffset.Now,
                    PocketChange = new decimal(0.01) * new decimal(i),
                    TimeSpent = TimeSpan.FromMinutes(60),
                    CarColor = Color.Red,
                    SomeObject = new object(),
                    SometimesNullObject = i % 2 == 0 ? null : new object()
                };

                autostat.Collect(record);
            }
            
            var autostat2 = new AutoStat<TestRecord>(keyName: "id");

            for (int i = 0; i < count; i++)
            {
                TestRecord record = new TestRecord()
                {
                    Name = "John" + i.ToString().PadLeft(6, '0'),
                    Weight = count - (i / (count - i)),
                    ID = i,
                    TransactionID = Guid.NewGuid(),
                    NetWorth = 5000,
                    SeenDate = DateTime.Now,
                    OtherDate = DateTimeOffset.Now,
                    PocketChange = new decimal(0.01) * new decimal(i),
                    TimeSpent = TimeSpan.FromMinutes(60),
                    CarColor = Color.Red,
                    SomeObject = new object(),
                    SometimesNullObject = i % 2 == 0 ? null : new object()
                };

                autostat2.Collect(record);
            }
            
            var comparedStats = autostat2.GetStatsComparedTo(autostat);

            Assert.True(comparedStats.Where(stat => stat.Name.Contains("Sample") && stat.IsDifferent && stat.MemberName == "TransactionID").Any());

            //...
        }

        [Fact]
        public void NoKeyTest()
        {
            var autostat = new AutoStat<TestRecord>();

            int count = 1_000;
            for (int i = 0; i < count; i++)
            {
                TestRecord record = new TestRecord()
                {
                    Name = "John" + i.ToString().PadLeft(6, '0'),
                    Weight = count - (i / (count - i)),
                    ID = i,
                    TransactionID = Guid.NewGuid(),
                    NetWorth = 5000,
                    SeenDate = DateTime.Now,
                    OtherDate = DateTimeOffset.Now,
                    PocketChange = new decimal(0.01) * new decimal(i),
                    TimeSpent = TimeSpan.FromMinutes(60),
                    CarColor = Color.Red,
                    SomeObject = new object(),
                    SometimesNullObject = i % 2 == 0 ? null : new object()
                };

                autostat.Collect(record);
            }

            autostat.GetStatsComparedTo(autostat);
        }

        [Fact]
        public void ResetTest()
        {
            var autostat = new AutoStat<TestRecord>();

            int count = 100;

            void CollectSomeRecords()
            {
                for (int i = 0; i < count; i++)
                {
                    TestRecord record = new TestRecord()
                    {
                        Name = "John" + i.ToString().PadLeft(6, '0'),
                        Weight = count - (i / (count - i)),
                        ID = i,
                        TransactionID = Guid.NewGuid(),
                        NetWorth = 5000,
                        SeenDate = DateTime.Now,
                        OtherDate = DateTimeOffset.Now,
                        PocketChange = new decimal(0.01) * new decimal(i),
                        TimeSpent = TimeSpan.FromMinutes(60),
                        CarColor = Color.Red,
                        SomeObject = new object(),
                        SometimesNullObject = i % 2 == 0 ? null : new object()
                    };

                    autostat.Collect(record);
                }
            }

            CollectSomeRecords();

            Assert.True(autostat.Count == 100);

            autostat.Reset();

            CollectSomeRecords();
            var stats2 = autostat.GetStats();

            Assert.True(autostat.Count == 100);

            Assert.True((stats2.Where(stat => stat.MemberName == "Name" && stat.Name == "Count").First() as CountStat).Count == count);
        }
    }
}
