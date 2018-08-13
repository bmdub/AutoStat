using BW.Diagnostics.StatCollection;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Text;

namespace BW.Diagnostics.AutoStat.Test
{
    class TestRecord
    {
        [AutoStat]
        public string Name { get; set; }
        public int Weight { get; set; }
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
        public object SometimesNullObject { get; set; }
    }

    enum Color
    {
        Red, Green, Blue,
    }
}
