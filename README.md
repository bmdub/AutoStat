# AutoStat
https://www.nuget.org/packages/AutoStat

AutoStat is a stat collector which collects statistics on objects in a stream (or a batch).  Time and memory complexity are linear, meaning that stat collection can be done on a stream in real-time, or on a batch of records in a reasonable amount of time.

Built-in stats:<br/>
- Count <br/>
- Default/Null Count<br/>
- Distinct Count (Estimated)<br/>
- Min/Max Value<br/>
- Mean, Standard Deviation<br/>
- N Most Frequent Occurrences (Estimated)<br/>
- Percentile Values (Estimated)<br/>
- Existence Comparison (Estimated, compares the existence of all unique values between 2 data sets)<br/>
- Sample Comparison (Estimated, compares the values between two data sets for key-matched records)<br/>

Note: This class is not thread-safe.

### Usage

```CSharp
using BW.Diagnostics.StatCollection;
using BW.Diagnostics.StatCollection.Stats; // If you want to make custom stats
``` 
Construct with the type of object you want to collect stats on:
```CSharp
var autoStat1 = new AutoStat<Person>(keyName: "id");
``` 
Run each record through:
```CSharp
autoStat1.Collect(record);
```
At any point, compute the current stats:
```CSharp
var recordStats1 = autoStat1.GetStats();
``` 
Output the stats to the console window:
```CSharp
Console.Write(recordStats1.ToTextTableFormat(Console.WindowWidth));
``` 
Note: Specifying a key property in the constructor is optional. Using a key will allow for the "Sample" comparison stat.

### Comparison Stats

You can compare the statistics (including an estimate of equality) of two streams as so:
```CSharp
var recordStats3 = autoStat1.GetStatsComparedTo(autoStat2)
```
This can be useful for determining if two sets have changed over time.
<br/>

You can select certain stats to highlight in the output, based on custom criteria:
```CSharp
var recordStats3 = autoStat1.GetStatsComparedTo(autoStat2)
  .HighlightWhen(stat => stat.DiffPct >= .30);
``` 
<br/>

## Alternate output formatting:

To output to .csv:
```CSharp
recordStats1.ToCsvFile("stats.csv");
```

To output to .csv, then open in Powershell (Windows only):
```CSharp
recordStats1.OpenCsvInPowershell("stats.csv");
```

## Choosing Class Members

By default, all public properties are chosen for stat collection.

To choose certain class members only, specify an attribute for those class members:
```CSharp
[AutoStat]
public long SerialNumber { get; set; }
```
Then, specify in configuration that AutoStat should only choose members with the attribute:
```CSharp
var config = new TestConfiguration(SelectionMode.Attribute);
var autoStat1 = new AutoStat<Host>(config);
```

## Choosing Stats
Stat collectors are responsible for collecting and reporting one or more stat values.  You can select the names of the 
specific collectors you want (default is Configuration.DefaultStatCollectors):
```CSharp
var config = new TestConfiguration(SelectionMode.All, new[] { "CountStatCollector", "DistinctStatCollector" });
var autoStat1 = new AutoStat<Host>(config);
```

## Filtering Output
You can also filter the output stats calculated from GetStats() or GetStatsComparedTo() by:
```CSharp
var recordStats1 = autoStat1.GetStats();
recordStats = recordStats.Where(stat => stat.MemberName == "SerialNumber").ToRecordStats();
```

## Custom Stats
To make your own stats, you must make an IStatCollector, IStat, and IComparedStat.
Optionally, you can subclass Configuration to pass in custom configuration values.
Then, you would add the custom stat collector to your AutoStat instance in configuration as so:
```CSharp
var config = new TestConfiguration(SelectionMode.All, Configuration.DefaultStatCollectors.Append("TestStatCollector"));
var autoStat1 = new AutoStat<Host>(config);
```
Code for a "Test" stat collector is defined below:
```CSharp


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
```


            






